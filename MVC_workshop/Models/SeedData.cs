using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC_workshop.Areas.Identity.Data;
using MVC_workshop.Data;

namespace MVC_workshop.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVC_workshopUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            MVC_workshopUser user = await UserManager.FindByEmailAsync("admin@mvcworkshop.com");
            if (user == null)
            {
                var User = new MVC_workshopUser();
                User.Email = "admin@mvcworkshop.com";
                User.UserName = "admin@mvcworkshop.com";
                string userPWD = "Admin995";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            // creating Teacher role     
            var x = await RoleManager.RoleExistsAsync("Teacher");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Teacher";
                await RoleManager.CreateAsync(role);
            }

            // creating Student role     
            x = await RoleManager.RoleExistsAsync("Student");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Student";
                await RoleManager.CreateAsync(role);
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVC_workshopContext(
               serviceProvider.GetRequiredService<
                   DbContextOptions<MVC_workshopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Courses.Any() || context.Enrollments.Any() || context.Students.Any() || context.Teachers.Any())
                {
                    return;
                }

                context.Teachers.AddRange(
                    new Teacher { FirstName = "Daniel", LastName = "Denkovski", Degree = "PhD", AcademicRank = "Assistant Professor", OfficeNumber = "99999", HireDate = DateTime.Parse("2010-05-20") },
                    new Teacher { FirstName = "Pero", LastName = "Latkoski", Degree = "PhD", AcademicRank = "Full Professor", OfficeNumber = "888888", HireDate = DateTime.Parse("2005-05-20") },
                    new Teacher { FirstName = "Bojana", LastName = "Velickovska", Degree = "MSc", AcademicRank = "Teaching Assistant", OfficeNumber = "777777", HireDate = DateTime.Parse("2018-05-20") },
                    new Teacher { FirstName = "Vladimir", LastName = "Atanasovski", Degree = "PhD", AcademicRank = "Full Professor", OfficeNumber = "7765477", HireDate = DateTime.Parse("2004-05-20") });

                context.SaveChanges();

                context.Courses.AddRange(new Course { Title = "RSWEB", Credits = 6, Semester = 6, Programme = "KTI,TKII", EducationLevel = "BSc", FirstTeacherId = 1, SecondTeacherId = 2 },
                                        new Course { Title = "MSAP", Credits = 6, Semester = 6, Programme = "KTI,TKII", EducationLevel = "BSc", FirstTeacherId = 2, SecondTeacherId = 1 },
                                        new Course { Title = "PSAA", Credits = 6, Semester = 4, Programme = "KTI,TKII", EducationLevel = "BSc", FirstTeacherId = 1, SecondTeacherId = 3 },
                                        new Course { Title = "TKM", Credits = 6, Semester = 6, Programme = "TKII", EducationLevel = "BSc", FirstTeacherId = 4, SecondTeacherId = 2 });


                context.SaveChanges();

                context.Students.AddRange(
                    new Student { StudentId = "11/2019", FirstName = "Anastasija", LastName = "Stefanovska", EnrollmentDate = DateTime.Parse("2019-9-11"), AcquiredCredits = 150, CurrentSemestar = 6, EducationLevel = "student" },
                    new Student { StudentId = "304/2018", FirstName = "Ivana", LastName = "Ivanova", EnrollmentDate = DateTime.Parse("2018-10-1"), AcquiredCredits = 168, CurrentSemestar = 8, EducationLevel = "student" },
                    new Student { StudentId = "1/2019", FirstName = "Emilija", LastName = "Chona", EnrollmentDate = DateTime.Parse("2019-9-15"), AcquiredCredits = 150, CurrentSemestar = 6, EducationLevel = "student" },
                    new Student { StudentId = "505/2019", FirstName = "Nikola", LastName = "Nikolovski", EnrollmentDate = DateTime.Parse("2019-9-10"), AcquiredCredits = 100, CurrentSemestar = 6, EducationLevel = "student" }
                 );
                context.SaveChanges();

                context.Enrollments.AddRange(
                    new Enrollment
                    {
                        // CourseId=context.Course.Single(d => d.Title == "RSWEB").Id,
                        // StudentId = context.Student.Single(d => d.StudentId == "11/2019").Id,
                        CourseId = 4,
                        StudentId = 1,
                        Semester = "leten",
                        Year = 3,
                        Grade = 10,
                        SeminalUrl = "je",
                        ProjectUrl = "je",
                        ExamPoints = 90,
                        SeminalPoints = 35,
                        ProjectPoints = 40,
                        AdditionalPoints = 15,
                        FinishDate = DateTime.Parse("2022-5-25")
                    },
                     new Enrollment
                     {
                         CourseId = 1,
                         StudentId = 1,
                         Semester = "leten",
                         Year = 3,
                         Grade = 10,
                         SeminalUrl = "je",
                         ProjectUrl = "je",
                         ExamPoints = 90,
                         SeminalPoints = 35,
                         ProjectPoints = 40,
                         AdditionalPoints = 15,
                         FinishDate = DateTime.Parse("2022-5-25")
                     },
                      new Enrollment
                      {
                          CourseId = 1,
                          StudentId = 2,
                          Semester = "leten",
                          Year = 3,
                          Grade = 10,
                          SeminalUrl = "je",
                          ProjectUrl = "je",
                          ExamPoints = 90,
                          SeminalPoints = 35,
                          ProjectPoints = 40,
                          AdditionalPoints = 15,
                          FinishDate = DateTime.Parse("2022-5-25")
                      },
                      new Enrollment
                      {
                          CourseId = 3,
                          StudentId = 3,
                          Semester = "leten",
                          Year = 4,
                          Grade = 10,
                          SeminalUrl = "je",
                          ProjectUrl = "je",
                          ExamPoints = 90,
                          SeminalPoints = 35,
                          ProjectPoints = 40,
                          AdditionalPoints = 15,
                          FinishDate = DateTime.Parse("2022-5-25")
                      },
                      new Enrollment
                      {
                          CourseId = context.Courses.Single(d => d.Title == "MSAP").Id,
                          StudentId = 1,
                          Semester = "leten",
                          Year = 3,
                          Grade = 10,
                          SeminalUrl = "je",
                          ProjectUrl = "je",
                          ExamPoints = 90,
                          SeminalPoints = 35,
                          ProjectPoints = 40,
                          AdditionalPoints = 15,
                          FinishDate = DateTime.Parse("2022-5-25")
                      }
                );
                context.SaveChanges();
            }
        }

    }
}
