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
    public class StudentsController : Controller
    {
        private readonly MVC_workshopContext _context;

        public StudentsController(MVC_workshopContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        // GET: Students
        public async Task<IActionResult> Index(string searchString)
        {
            var students = from s in _context.Students
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.FirstName!.Contains(searchString) || s.LastName!.Contains(searchString) || s.StudentId!.Contains(searchString));
            }
            return View(await students.ToListAsync());
        }

        // GET: Students/Details/5
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Details(int? id, string? userID)
        {
            if (userID != null)
            {
                var st = await _context.Students.FirstOrDefaultAsync(x => x.userId == userID);
                id = st.Id;
            }
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            Student_EditPicture vm = new Student_EditPicture
            {
                student = student,
                pictureName = student.Picture
            };
            return View(vm);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel,Picture")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            Student_EditPicture vm = new Student_EditPicture
            {
                student = student,
                pictureName = student.Picture
            };
            return View(vm);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student_EditPicture vm)
        {
            if (id != vm.student.Id)
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
                        vm.student.Picture = uniqueFileName;
                    }
                    else
                    {
                        vm.student.Picture = vm.pictureName;
                    }
                    _context.Update(vm.student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(vm.student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = vm.student.Id });
            }
            return View(vm);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StudentsCourse(int? id, string searchString)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course= await _context.Courses.FindAsync(id);
            ViewBag.Message = course.Title;
            IQueryable<Student> studentsq = _context.Enrollments.Where(x => x.CourseId == id).Select(x => x.Student);
            if (!String.IsNullOrEmpty(searchString))
            {
                studentsq = studentsq.Where(s => s.FirstName!.Contains(searchString) || s.LastName!.Contains(searchString) || s.StudentId!.Contains(searchString));
            }
            return View(await studentsq.ToListAsync());
            await _context.SaveChangesAsync();
            return View(await studentsq.ToListAsync());

        }
       
        private string UploadedFile(Student_EditPicture vm)
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
