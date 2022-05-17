#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_workshop.Data;
using MVC_workshop.Models;
using MVC_workshop.ViewModels;

namespace MVC_workshop.Controllers
{
    public class TeachersController : Controller
    {
        private readonly MVC_workshopContext _context;

        public TeachersController(MVC_workshopContext context)
        {
            _context = context;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString)
        {
                var teachers = from t in _context.Teachers.Include(m => m.CoursesF)
                    .Include(m => m.CoursesS)
                               select t;
                if (!String.IsNullOrEmpty(searchString))
                {
                    teachers = teachers.Where(s => s.FirstName!.Contains(searchString) || s.LastName!.Contains(searchString) || s.Degree!.Contains(searchString) || s.AcademicRank!.Contains(searchString));
                }
                return View(await teachers.ToListAsync());
            
        }

        // GET: Teachers/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int? id,string? userID)
        {
            if (userID != null)
            {
                var tc = await _context.Teachers.FirstOrDefaultAsync(x => x.userId == userID);
                id = tc.Id;
            }
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            Teacher_EditPicture vm = new Teacher_EditPicture
            {
                teacher = teacher,
                pictureName = teacher.Picture
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,Picture")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            Teacher_EditPicture vm = new Teacher_EditPicture
            {
                teacher = teacher,
                pictureName = teacher.Picture
            };
            return View(vm);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher_EditPicture vm)
        {
            if (id != vm.teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (vm.pictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(vm);
                        vm.teacher.Picture = uniqueFileName;
                    }
                    else
                    {
                        vm.teacher.Picture = vm.pictureName;
                    }

                    _context.Update(vm.teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(vm.teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = vm.teacher.Id });
            }
            return View(vm);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            IQueryable<Course> courses = _context.Courses.AsQueryable();
            IQueryable<Course> courses1=courses.Where(s=>s.FirstTeacherId == id);
            IQueryable<Course> courses2 = courses.Where(s => s.SecondTeacherId == id);
            foreach(var course in courses1)
            {
                course.FirstTeacherId = null;
            }
            foreach(var course in courses2)
            {
                course.SecondTeacherId = null;
            }
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
        private string UploadedFile(Teacher_EditPicture vm)
        {
            string uniqueFileName = null;

            if (vm.pictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(vm.pictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vm.pictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }

    }
}
