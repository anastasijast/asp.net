using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public int Semester { get; set; }

        [Display(Name = "Programme")]
        [StringLength(100)]
        public string? Programme { get; set; }

        [Display(Name = "Education Level")]
        [StringLength(25)]
        public string? EducationLevel { get; set; }

        public int? FirstTeacherId { get; set; }
        [Display(Name = "First Teacher")]
        public Teacher? FirstTeacher { get; set; }
        public int? SecondTeacherId { get; set; }

        [Display(Name = "Second Teacher")]
        public Teacher? SecondTeacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

        internal object Include(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
