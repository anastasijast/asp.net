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
    public class EnrollmentsController : Controller
    {
        private readonly MVC_workshopContext _context;

        public EnrollmentsController(MVC_workshopContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var mVC_workshopContext = _context.Enrollments.Include(e => e.Course).Include(e => e.Student);
            return View(await mVC_workshopContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        [Authorize(Roles = "Admin,Student,Teacher")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,CourseId,StudentId")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,CourseId,StudentId")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> StudentsEnCourse(int? id,  string? tname, int year)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Courses
                .FindAsync(id);
            string[] tnames = tname.Split(" ");
            var teacher = _context.Teachers.Where(x => x.FirstName==tnames[0] && x.LastName==tnames[1]).First();
            ViewBag.teacher = teacher.FullName;
            ViewBag.course = course.Title;
            var enrollment = _context.Enrollments.Where(x => x.CourseId==id )
                .Include(e => e.Course)
            .Include(e => e.Student);
            await _context.SaveChangesAsync();
            IQueryable<int?> yearsq = _context.Enrollments.OrderBy(m => m.Year).Select(m => m.Year).Distinct();
            IQueryable<Enrollment> enrollmentq = enrollment.AsQueryable();
            if (year != null && year != 0)
            {
                enrollmentq = enrollmentq.Where(x => x.Year == year);
            }
            else
            {
                enrollmentq = enrollmentq.Where(x => x.Year == yearsq.Max());
            }

            if (enrollment == null)
            {
                return NotFound();
            }
            EnrollmentVM vm = new EnrollmentVM
            {
                Enrollments = await enrollmentq.ToListAsync(),
                Years = new SelectList(await yearsq.ToListAsync())
            };

            return View(vm);
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CoursesStudent(int? id, string? userID)
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
            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            IQueryable<Enrollment> enrollments = _context.Enrollments.Where(x => x.StudentId == student.Id)
                .Include(e => e.Course)
                .Include(e => e.Student);
            var studentID = await _context.Students.FindAsync(id);
            ViewBag.Student = student.FullName;
            await _context.SaveChangesAsync();
            return View(await enrollments.ToListAsync());
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enrollment = _context.Enrollments.Where(x => x.Id == id).Include(x => x.Student).Include(x => x.Course).First();
            IQueryable<Enrollment> enrollmentq = _context.Enrollments.AsQueryable();
            enrollmentq = enrollmentq.Where(x => x.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }
            StudentEditVM vm = new StudentEditVM
            {
                enrollment = await enrollmentq.Include(x => x.Student).Include(x => x.Course).FirstAsync(),
                seminalUrl = enrollment.SeminalUrl

            };
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(vm);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentEdit(int id, StudentEditVM vm)
        {
            if (id != vm.enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (vm.seminalFile != null)
                    {
                        string uniqueFileName = UploadedFile(vm);
                        vm.enrollment.SeminalUrl = uniqueFileName;
                    }
                    else
                    {
                        vm.enrollment.SeminalUrl = vm.seminalUrl;
                    }

                    _context.Update(vm.enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(vm.enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("CoursesStudent", new { id = vm.enrollment.StudentId });
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", vm.enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", vm.enrollment.StudentId);
            return View(vm);
        }
        private string UploadedFile(StudentEditVM vm)
        {
            string uniqueFileName = null;

            if (vm.seminalFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/seminals");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(vm.seminalFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    vm.seminalFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherEdit(int? id, string tname)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewBag.teacher = tname;
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherEdit(int id,string tname, [Bind("Id,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,CourseId,StudentId")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            string teacher = tname;
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("StudentsEnCourse", new { id = enrollment.CourseId,tname=teacher, year = enrollment.Year });
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
    }
}
