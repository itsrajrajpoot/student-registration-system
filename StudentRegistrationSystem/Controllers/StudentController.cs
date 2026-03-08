using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistrationSystem.DTOs;
using StudentRegistrationSystem.Models;
using StudentRegistrationSystem.Services;

namespace StudentRegistrationSystem.Controllers;

[Authorize] 
public class StudentController(
    IStudentService service,
    IMapper mapper,
    ILogger<StudentController> logger) : Controller
{
    
    [Authorize(Roles = "SystemAdmin,ViewStudent")]
    public async Task<IActionResult> Index(
        string? search,
        string? course,
        string? status,
        int page = 1,
        int pageSize = 5)
    {
        var students = await service.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            students = students.Where(s =>
                s.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        }


        if (!string.IsNullOrWhiteSpace(course))
        {
            students = students.Where(s =>
                s.Course.Equals(course, StringComparison.OrdinalIgnoreCase));
        }

        
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                students = students.Where(s => s.IsActive);

            if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                students = students.Where(s => !s.IsActive);
        }

        var totalCount = students.Count();

        var pagedData = students
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.Status = status;

        return View(mapper.Map<IEnumerable<StudentDto>>(pagedData));
    }

    
    [Authorize(Roles = "SystemAdmin,AddStudent")]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SystemAdmin,AddStudent")]


    public async Task<IActionResult> Create(StudentDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var students = await service.GetAllAsync();

        var existingStudent = students
            .FirstOrDefault(s => s.Email.ToLower() == dto.Email.ToLower());

        if (existingStudent != null && !existingStudent.IsActive)
        {
            existingStudent.FullName = dto.FullName;
            existingStudent.RollNumber = dto.RollNumber;
            existingStudent.Course = dto.Course;
            existingStudent.Gender = dto.Gender;
            existingStudent.IsActive = true;

            await service.UpdateAsync(existingStudent);

            TempData["Success"] = "Student added successfully.";

            return RedirectToAction(nameof(Index));
        }

        
        if (existingStudent != null && existingStudent.IsActive)
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View(dto);
        }

        
        var student = mapper.Map<Student>(dto);

        if (dto.ProfilePhoto != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePhoto.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePhoto.CopyToAsync(stream);

            student.ProfilePhotoPath = "/images/" + fileName;
        }

        await service.CreateAsync(student);

        TempData["Success"] = "Student added successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "SystemAdmin,EditStudent")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var student = await service.GetByIdAsync(id);

        if (student == null)
            return NotFound();

        var dto = mapper.Map<StudentDto>(student);

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SystemAdmin,EditStudent")]
    public async Task<IActionResult> Edit(StudentDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var student = await service.GetByIdAsync(dto.Id);
        if (student == null)
            return NotFound();

        mapper.Map(dto, student);

        if (dto.ProfilePhoto != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePhoto.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePhoto.CopyToAsync(fileStream);

            student.ProfilePhotoPath = "/images/" + uniqueFileName;
        }

        await service.UpdateAsync(student);

        TempData["Success"] = "Student updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SystemAdmin,DeleteStudent")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);

        TempData["Success"] = "Student deleted successfully!";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<JsonResult> CheckEmail(string email)
    {
        var students = await service.GetAllAsync();

        bool exists = students.Any(s => s.Email.ToLower() == email.ToLower());

        return Json(!exists); 
    }
}