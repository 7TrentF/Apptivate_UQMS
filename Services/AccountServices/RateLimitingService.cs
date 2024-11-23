using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using static Apptivate_UQMS_WebApp.Models.Account;

namespace Apptivate_UQMS_WebApp.Services.AccountServices
{
    // RateLimitingService.cs
    public class RateLimitingService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private const int MaxAttempts = 5;
        private const int LockoutMinutes = 15;
        private readonly ApplicationDbContext _context;
        private const int AttemptWindowMinutes = 15;

        public RateLimitingService(ApplicationDbContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;

            _cache = cache;
            _configuration = configuration;
        }

        public async Task<(bool isAllowed, int remainingAttempts, DateTime? lockoutEnd)> CheckLoginAttempt(string ipAddress, string email)
        {
            // Clean up old attempts
            await CleanupOldAttempts();

            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(x => x.IpAddress == ipAddress && x.Email == email);

            // If no existing attempt record or window expired, create/reset one
            if (attempt == null || attempt.FirstAttemptAt.AddMinutes(AttemptWindowMinutes) < DateTime.Now)
            {
                if (attempt == null)
                {
                    attempt = new LoginAttempt
                    {
                        IpAddress = ipAddress,
                        Email = email,
                        AttemptCount = 1,
                        FirstAttemptAt = DateTime.Now
                    };
                    _context.LoginAttempts.Add(attempt);
                }
                else
                {
                    attempt.AttemptCount = 1;
                    attempt.FirstAttemptAt = DateTime.Now;
                    attempt.LockoutEnd = null;
                }
                await _context.SaveChangesAsync();
                return (true, MaxAttempts - 1, null);
            }

            // Check if account is locked
            if (attempt.LockoutEnd.HasValue && attempt.LockoutEnd.Value > DateTime.Now)
            {
                return (false, 0, attempt.LockoutEnd);
            }

            // Increment attempt count
            attempt.AttemptCount++;
            var remainingAttempts = MaxAttempts - attempt.AttemptCount;

            // Check if should be locked out
            if (attempt.AttemptCount >= MaxAttempts)
            {
                attempt.LockoutEnd = DateTime.Now.AddMinutes(LockoutMinutes);
                await _context.SaveChangesAsync();
                return (false, 0, attempt.LockoutEnd);
            }

            await _context.SaveChangesAsync();
            return (true, remainingAttempts, null);
        }

        public async Task ResetLoginAttempts(string ipAddress, string email)
        {
            var attempt = await _context.LoginAttempts
                .FirstOrDefaultAsync(x => x.IpAddress == ipAddress && x.Email == email);

            if (attempt != null)
            {
                _context.LoginAttempts.Remove(attempt);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CleanupOldAttempts()
        {
            var cutoff = DateTime.Now.AddMinutes(-AttemptWindowMinutes);
            var oldAttempts = await _context.LoginAttempts
                .Where(x => x.FirstAttemptAt < cutoff && (!x.LockoutEnd.HasValue || x.LockoutEnd < DateTime.Now))
                .ToListAsync();

            if (oldAttempts.Any())
            {
                _context.LoginAttempts.RemoveRange(oldAttempts);
                await _context.SaveChangesAsync();
            }
        }
    }
}