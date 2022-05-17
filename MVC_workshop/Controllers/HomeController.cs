using Microsoft.AspNetCore.Mvc;
using MVC_workshop.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using MVC_workshop.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_workshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MVC_workshop.ViewModels;

namespace MVC_workshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MVC_workshopContext _context;
        private readonly UserManager<MVC_workshopUser> _userManager;


        public HomeController(ILogger<HomeController> logger, MVC_workshopContext context, UserManager<MVC_workshopUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "Admin")]

        public IActionResult RegisterTeacher(string userID)
        {
            if (userID == null)
            {
                return NotFound();
            }
            ViewBag.User = userID;
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "FullName");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterTeacher(string? userID, int? Id)
        {
            if(userID == null)
            {
                return NotFound();
            }
            var teacher = _context.Teachers.Where(x => x.Id == Id).First();
            teacher.userId = userID;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterStudent(string userID)
        {
            if (userID == null)
            {
                return NotFound();
            }
            ViewBag.User = userID;
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName");
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(string? userID, int? Id)
        {
            if (userID == null)
            {
                return NotFound();
            }
            var student = _context.Students.Where(x => x.Id == Id).First();
            student.userId = userID;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AllUsers()
        {
            var users = _userManager.Users.Where(x => !x.Email.Contains("admin"));
            return View(users);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UserEdit(string id)
        {
            var user=await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }
            var model = new EditUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(EditUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{model.Id}'.");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result= await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("AllUsers");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            
            return View(model);
        }
        
    }
}