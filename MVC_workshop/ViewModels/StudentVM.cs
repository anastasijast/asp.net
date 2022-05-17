using MVC_workshop.Models;

namespace MVC_workshop.ViewModels
{
    public class StudentVM
    {
        public IList<Student> Students { get; set; }
        public string FullName { get; set; }
        public int StudentId { get; set; }


    }
}
