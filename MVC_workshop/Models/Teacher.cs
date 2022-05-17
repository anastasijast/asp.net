using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "Degree")]
        public string? Degree { get; set; }

        [StringLength(25)]
        [Display(Name = "Academic Rank")]
        public string? AcademicRank { get; set; }

        [StringLength(10)]
        [Display(Name = "Office Number")]
        public string? OfficeNumber { get; set; }

        public string? Picture { get; set; }
        public string? userId { get; set; }


        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
        public ICollection<Course>? CoursesF { get; set; }
        public ICollection<Course>? CoursesS { get; set; }
    }
}
