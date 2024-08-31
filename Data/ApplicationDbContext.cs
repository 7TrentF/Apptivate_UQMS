using Apptivate_UQMS_WebApp.Models;
using Microsoft.EntityFrameworkCore;

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
