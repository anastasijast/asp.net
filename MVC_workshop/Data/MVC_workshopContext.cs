#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_workshop.Areas.Identity.Data;
using MVC_workshop.Models;


namespace MVC_workshop.Data
{
    public class MVC_workshopContext : IdentityDbContext<MVC_workshopUser>
    {
        public MVC_workshopContext(DbContextOptions<MVC_workshopContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           // builder.Entity<Student>().ToTable("Student");
            //builder.Entity<Course>().ToTable("Course");
            //builder.Entity<Enrollment>().ToTable("Enrollment");
            //builder.Entity<Teacher>().ToTable("Teacher");

            builder.Entity<Course>()
               .HasOne<Teacher>(f => f.FirstTeacher)
               .WithMany(c => c.CoursesF)
               .HasForeignKey(f => f.FirstTeacherId);

            builder.Entity<Course>()
                .HasOne<Teacher>(f => f.SecondTeacher)
                .WithMany(c => c.CoursesS)
                .HasForeignKey(f => f.SecondTeacherId);

            builder.Entity<Enrollment>()
                .HasOne<Student>(s => s.Student)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(s => s.StudentId);

            builder.Entity<Enrollment>()
                .HasOne<Course>(c => c.Course)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(c => c.CourseId);


        }

    }
}
