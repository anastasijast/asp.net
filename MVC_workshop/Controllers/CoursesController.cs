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
using Microsoft.AspNetCore.Identity;
using MVC_workshop.Areas.Identity.Data;


namespace MVC_workshop.Controllers
{
    public class CoursesController : Controller
    {
        private readonly MVC_workshopContext _context;

        public CoursesController(MVC_workshopContext context)
        {
            _context = context;
        }

        // GET: Courses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString)
        {
            var courses = from c in _context.Courses.Include(m => m.Enrollments)
                                               .ThenInclude(m => m.Student)
                          select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                int br;
                bool isNumber = int.TryParse(searchString, out br);
                if (isNumber)
                {
                    courses = courses.Where(s => s.Semester == br);
                }
                else
                {
                    courses = courses.Where(s => s.Title!.Contains(searchString) || s.Programme!.Contains(searchString));
                }
            }
            return View(await courses.ToListAsync());
        }
        /*public async Task<IActionResult> Index(int Id)
        {
            var courses = from c in _context.Courses
                          select c;
            courses = courses.Where(s => s.FirstTeacherId == Id || s.SecondTeacherId==Id);
            return View(await courses.ToListAsync());
        }*/
        // GET: Courses/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FirstName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Courses.Where(x => x.Id == id).Include(x => x.Enrollments).First();
            IQueryable<Course> coursesq = _context.Courses.AsQueryable();
            coursesq = coursesq.Where(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var students = _context.Students.AsEnumerable();
            students = students.OrderBy(s => s.FullName);
            EnrollMoreStudents vm = new EnrollMoreStudents
            {
                // course = course,
                course = await coursesq.Include(x => x.FirstTeacher).Include(x => x.SecondTeacher).FirstAsync(),
                studentsList = new MultiSelectList(students, "Id", "FullName"),
                selectedStudents = course.Enrollments.Select(x => x.StudentId)
            };
            ViewData["FirstTeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teachers, "Id", "FullName", course.SecondTeacherId);
            ViewBag.Message = vm.course.Title;
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnrollMoreStudents vm)
        {
            if (id != vm.course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vm.course);
                    await _context.SaveChangesAsync();
                    var course = await _context.Courses.FindAsync(id);
                    string sem;
                    if (course.Semester % 2 == 0)
                    {
                        sem = "leten";
                    }
                    else
                    {
                        sem = "zimski";
                    }
                    IEnumerable<int> listStudents = vm.selectedStudents;
                    if (listStudents != null)
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => !listStudents.Contains(s.Student.Id) && s.CourseId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        IEnumerable<int> exist = _context.Enrollments.Where(x => listStudents.Contains(x.Student.Id) && x.CourseId == id).Select(x => x.Student.Id);
                        IEnumerable<int> newEn = listStudents.Where(x => !exist.Contains(x));

                        foreach (int student in newEn)
                            _context.Add(new Enrollment { Semester = sem, Year = (int)vm.Year, StudentId = student, CourseId = id });

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        IQueryable<Enrollment> toBeRemoved = _context.Enrollments.Where(s => s.CourseId == id);
                        _context.Enrollments.RemoveRange(toBeRemoved);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(vm.course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CoursesTeacher(int? id,string? userID, string searchString)
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
            var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == id);
            ViewBag.Teacher = teacher.FullName;
            IQueryable<Course> coursesq = _context.Courses.Where(x => x.FirstTeacherId == id || x.SecondTeacherId==id);
            if (!String.IsNullOrEmpty(searchString))
            {
                int br;
                bool isNumber = int.TryParse(searchString, out br);
                if (isNumber)
                {
                    coursesq = coursesq.Where(s => s.Semester == br);
                }
                else
                {
                    coursesq = coursesq.Where(s => s.Title!.Contains(searchString) || s.Programme!.Contains(searchString));
                }
            }
            await _context.SaveChangesAsync();
            return View(await coursesq.ToListAsync());

        }
    }
}
