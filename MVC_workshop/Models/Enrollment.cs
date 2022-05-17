using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Display(Name = "Semester")]
        [StringLength(10)]
        public string? Semester { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Grade")]
        public int? Grade { get; set; }

        [Display(Name = "Seminal Url")]
        [StringLength(255)]
        public string? SeminalUrl { get; set; }

        [Display(Name = "Project Url")]
        [StringLength(255)]
        public string? ProjectUrl { get; set; }

        [Display(Name = "Exam Points")]
        public int? ExamPoints { get; set; }

        [Display(Name = "Seminal Points")]
        public int? SeminalPoints { get; set; }

        [Display(Name = "Project Points")]
        public int? ProjectPoints { get; set; }

        [Display(Name = "Additional Points")]
        public int? AdditionalPoints { get; set; }

        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }

        [Required]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
