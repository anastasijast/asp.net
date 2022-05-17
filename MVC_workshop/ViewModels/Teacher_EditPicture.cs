using MVC_workshop.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_workshop.ViewModels
{
    public class Teacher_EditPicture
    {
        public Teacher? teacher { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? pictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? pictureName { get; set; }
    }
}
