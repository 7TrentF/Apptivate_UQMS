using Apptivate_UQMS_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.QueryModel;

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
        public DbSet<Position> Positions { get; set; }
        public DbSet<AdminDetail> AdminDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<DepartmentCourse> DepartmentCourses { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<QueryAssignment> QueryAssignments { get; set; }
        public DbSet<QueryDocument> QueryDocuments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<QueryCategory> QueryCategories { get; set; }
        public DbSet<QueryType> QueryTypes { get; set; }


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

            // Query and StudentDetail relationship
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Student)
                .WithMany(s => s.Queries)
                .HasForeignKey(q => q.StudentID);

            // Query and Department relationship
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Department)
                .WithMany(d => d.Queries)
                .HasForeignKey(q => q.DepartmentID);

            // Query and Course relationship
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Queries)
                .HasForeignKey(q => q.CourseID);

            // Query and Module relationship
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Module)
                .WithMany(m => m.Queries)
                .HasForeignKey(q => q.ModuleID);

            // QueryDocuments relationship
            modelBuilder.Entity<QueryDocument>()
                .HasOne(qd => qd.Query)
                .WithMany(q => q.QueryDocuments)
                .HasForeignKey(qd => qd.QueryID);

            // Feedback relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Query)
                .WithOne(q => q.Feedback)
                .HasForeignKey<Feedback>(f => f.QueryID);

            // QueryAssignments relationship
            modelBuilder.Entity<QueryAssignment>()
                .HasOne(qa => qa.Query)
                .WithMany(q => q.QueryAssignments)
                .HasForeignKey(qa => qa.QueryID);

            modelBuilder.Entity<QueryAssignment>()
                .HasOne(qa => qa.Staff)
                .WithMany(s => s.QueryAssignments)
                .HasForeignKey(qa => qa.StaffID);

            // Query and QueryCategory relationship
            modelBuilder.Entity<Query>()
                .HasOne(q => q.Category)
                .WithMany(c => c.Queries)
                .HasForeignKey(q => q.CategoryID);


        }
        public DbSet<Apptivate_UQMS_WebApp.Models.Users> Users_1 { get; set; } = default!;

    }
}

