using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_workshop.Models;

namespace MVC_workshop.ViewModels
{
    public class EnrollMoreStudents
    {
        public Course course { get; set; }

        public IEnumerable<int>? selectedStudents { get; set; }

        public IEnumerable<SelectListItem>? studentsList { get; set; }

        public int? Year { get; set; }
    }
}
