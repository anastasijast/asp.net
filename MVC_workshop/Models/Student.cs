using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "Student Id")]
        public string StudentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime? EnrollmentDate { get; set; }

        [Display(Name = "Acquired Credits")]
        public int? AcquiredCredits { get; set; }

        [Display(Name = "Current Semestar")]
        public int? CurrentSemestar { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }

        public string? Picture { get; set; }
        public string? userId { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
