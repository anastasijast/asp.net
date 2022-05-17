using MVC_workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.ViewModels
{
    public class StudentEditVM
    {
        public Enrollment enrollment { get; set; }
        public string? seminalUrl { get; set; }
        [Display(Name="Seminal File")]
        public IFormFile? seminalFile { get; set; }
    }
}
