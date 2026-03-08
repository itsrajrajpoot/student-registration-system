using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationSystem.Services;

namespace StudentRegistrationSystem.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IStudentService _studentService;

    public DashboardController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        var students = await _studentService.GetAllAsync();

        
        ViewBag.TotalStudents = students.Count();


        ViewBag.ActiveStudents = students.Count(s => s.IsActive);

        
        ViewBag.InactiveStudents = students.Count(s => !s.IsActive);

        
        ViewBag.TotalCourses = students
            .Select(s => s.Course)
            .Distinct()
            .Count();

        return View();
    }
}