using MVC_workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.ViewModels
{
    public class Student_EditPicture
    {
        public Student? student { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? pictureFile { get; set; }

        [Display(Name = "Picture")]
        public string? pictureName { get; set; }
    }
}
