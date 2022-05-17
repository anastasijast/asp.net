using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_workshop.Models;

namespace MVC_workshop.ViewModels
{
    public class EnrollmentVM
    {
        public IList<Enrollment> Enrollments { get; set; }

        public SelectList Years { get; set; }
        public int Year { get; set; }
    }
}
