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
        public DbSet<User> Users { get; set; }
        public DbSet<StudentDetail> StudentDetails { get; set; }
        public DbSet<StaffDetail> StaffDetails { get; set; }
        public DbSet<AdminDetail> AdminDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<DepartmentCourse> DepartmentCourses { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring many-to-many relationships
            modelBuilder.Entity<DepartmentCourse>()
                .HasKey(dc => new { dc.DepartmentID, dc.CourseID });

            modelBuilder.Entity<DepartmentCourse>()
                .HasOne(dc => dc.Department)
                .WithMany(d => d.DepartmentCourses)
                .HasForeignKey(dc => dc.DepartmentID);

            modelBuilder.Entity<DepartmentCourse>()
                .HasOne(dc => dc.Course)
                .WithMany(c => c.DepartmentCourses)
                .HasForeignKey(dc => dc.CourseID);

            modelBuilder.Entity<CourseModule>()
                .HasKey(cm => new { cm.CourseID, cm.ModuleID });

            modelBuilder.Entity<CourseModule>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.CourseModules)
                .HasForeignKey(cm => cm.CourseID);

            modelBuilder.Entity<CourseModule>()
                .HasOne(cm => cm.Module)
                .WithMany(m => m.CourseModules)
                .HasForeignKey(cm => cm.ModuleID);
        }
    }
}

