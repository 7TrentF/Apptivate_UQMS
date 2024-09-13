﻿// <auto-generated />
using System;
using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+AdminDetail", b =>
                {
                    b.Property<int>("AdminID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdminID"));

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("AdminID");

                    b.HasIndex("UserID");

                    b.ToTable("AdminDetails");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseID"));

                    b.Property<string>("CourseCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CourseID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+CourseModule", b =>
                {
                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<int>("ModuleID")
                        .HasColumnType("int");

                    b.Property<int>("CourseModuleID")
                        .HasColumnType("int");

                    b.HasKey("CourseID", "ModuleID");

                    b.HasIndex("ModuleID");

                    b.ToTable("CourseModules");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentID"));

                    b.Property<string>("DepartmentName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DepartmentID");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+DepartmentCourse", b =>
                {
                    b.Property<int>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentCourseID")
                        .HasColumnType("int");

                    b.HasKey("DepartmentID", "CourseID");

                    b.HasIndex("CourseID");

                    b.ToTable("DepartmentCourses");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Module", b =>
                {
                    b.Property<int>("ModuleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ModuleID"));

                    b.Property<string>("ModuleCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModuleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ModuleID");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Position", b =>
                {
                    b.Property<int>("PositionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PositionID"));

                    b.Property<string>("PositionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PositionID");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StaffDetail", b =>
                {
                    b.Property<int>("StaffID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffID"));

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PositionID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int?>("YearGroupTeaching")
                        .HasColumnType("int");

                    b.HasKey("StaffID");

                    b.HasIndex("PositionID");

                    b.HasIndex("UserID");

                    b.ToTable("StaffDetails");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StudentDetail", b =>
                {
                    b.Property<int>("StudentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentID"));

                    b.Property<string>("Course")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("StudentID");

                    b.HasIndex("UserID");

                    b.ToTable("StudentDetails");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirebaseUID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.DummyTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DummyTables");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+Feedback", b =>
                {
                    b.Property<int>("FeedbackID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackID"));

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QueryID")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("FeedbackID");

                    b.HasIndex("QueryID")
                        .IsUnique();

                    b.HasIndex("StudentID");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+Query", b =>
                {
                    b.Property<int>("QueryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QueryID"));

                    b.Property<int?>("CategoryID")
                        .HasColumnType("int");

                    b.Property<int?>("CourseID")
                        .HasColumnType("int");

                    b.Property<int?>("DepartmentID")
                        .HasColumnType("int");

                    b.Property<int?>("ModuleID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ResolvedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("QueryID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("CourseID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("ModuleID");

                    b.HasIndex("StudentID");

                    b.ToTable("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryAssignment", b =>
                {
                    b.Property<int>("AssignmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentID"));

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("QueryID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ResolutionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StaffID")
                        .HasColumnType("int");

                    b.HasKey("AssignmentID");

                    b.HasIndex("QueryID");

                    b.HasIndex("StaffID");

                    b.ToTable("QueryAssignments");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QueryTypeID")
                        .HasColumnType("int");

                    b.HasKey("CategoryID");

                    b.HasIndex("QueryTypeID");

                    b.ToTable("QueryCategories");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryDocument", b =>
                {
                    b.Property<int>("DocumentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentID"));

                    b.Property<string>("DocumentName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DocumentPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QueryID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("DocumentID");

                    b.HasIndex("QueryID");

                    b.ToTable("QueryDocuments");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryType", b =>
                {
                    b.Property<int>("QueryTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QueryTypeID"));

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QueryTypeID");

                    b.ToTable("QueryTypes");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+AdminDetail", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+User", "User")
                        .WithMany("AdminDetails")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+CourseModule", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Course", "Course")
                        .WithMany("CourseModules")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Module", "Module")
                        .WithMany("CourseModules")
                        .HasForeignKey("ModuleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Module");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+DepartmentCourse", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Course", "Course")
                        .WithMany("DepartmentCourses")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Department", "Department")
                        .WithMany("DepartmentCourses")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StaffDetail", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Position", "Position")
                        .WithMany("StaffDetails")
                        .HasForeignKey("PositionID");

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+User", "User")
                        .WithMany("StaffDetails")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Position");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StudentDetail", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+User", "User")
                        .WithMany("StudentDetails")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+Feedback", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.QueryModel+Query", "Query")
                        .WithOne("Feedback")
                        .HasForeignKey("Apptivate_UQMS_WebApp.Models.QueryModel+Feedback", "QueryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+StudentDetail", "Student")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Query");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+Query", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.QueryModel+QueryCategory", "Category")
                        .WithMany("Queries")
                        .HasForeignKey("CategoryID");

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Course", "Course")
                        .WithMany("Queries")
                        .HasForeignKey("CourseID");

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Department", "Department")
                        .WithMany("Queries")
                        .HasForeignKey("DepartmentID");

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+Module", "Module")
                        .WithMany("Queries")
                        .HasForeignKey("ModuleID");

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+StudentDetail", "Student")
                        .WithMany("Queries")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Course");

                    b.Navigation("Department");

                    b.Navigation("Module");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryAssignment", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.QueryModel+Query", "Query")
                        .WithMany("QueryAssignments")
                        .HasForeignKey("QueryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Apptivate_UQMS_WebApp.Models.Account+StaffDetail", "Staff")
                        .WithMany("QueryAssignments")
                        .HasForeignKey("StaffID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Query");

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryCategory", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.QueryModel+QueryType", "QueryType")
                        .WithMany("QueryCategories")
                        .HasForeignKey("QueryTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QueryType");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryDocument", b =>
                {
                    b.HasOne("Apptivate_UQMS_WebApp.Models.QueryModel+Query", "Query")
                        .WithMany("QueryDocuments")
                        .HasForeignKey("QueryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Query");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Course", b =>
                {
                    b.Navigation("CourseModules");

                    b.Navigation("DepartmentCourses");

                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Department", b =>
                {
                    b.Navigation("DepartmentCourses");

                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Module", b =>
                {
                    b.Navigation("CourseModules");

                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+Position", b =>
                {
                    b.Navigation("StaffDetails");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StaffDetail", b =>
                {
                    b.Navigation("QueryAssignments");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+StudentDetail", b =>
                {
                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.Account+User", b =>
                {
                    b.Navigation("AdminDetails");

                    b.Navigation("StaffDetails");

                    b.Navigation("StudentDetails");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+Query", b =>
                {
                    b.Navigation("Feedback");

                    b.Navigation("QueryAssignments");

                    b.Navigation("QueryDocuments");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryCategory", b =>
                {
                    b.Navigation("Queries");
                });

            modelBuilder.Entity("Apptivate_UQMS_WebApp.Models.QueryModel+QueryType", b =>
                {
                    b.Navigation("QueryCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
