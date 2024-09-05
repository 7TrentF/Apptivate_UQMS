using Apptivate_UQMS_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.Account;

namespace Apptivate_UQMS_WebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

      

        public DbSet<DummyTable> DummyTables { get; set; }
    }
}

