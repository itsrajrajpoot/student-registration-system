using Microsoft.EntityFrameworkCore;
using StudentRegistrationSystem.Data;
using StudentRegistrationSystem.Models;

namespace StudentRegistrationSystem.Repositories;

public class StudentRepository(ApplicationDbContext context) : IStudentRepository
{
    public async Task<IEnumerable<Student>> GetAllAsync()
        => await context.Students.ToListAsync();

    public async Task<Student?> GetByIdAsync(Guid id)
        => await context.Students.FindAsync(id);

    public async Task AddAsync(Student student)
        => await context.Students.AddAsync(student);

    public void Update(Student student)
        => context.Students.Update(student);

    public async Task SoftDeleteAsync(Student student)
    {
        student.IsDeleted = true;
        context.Students.Update(student);
        await Task.CompletedTask;
    }

    public async Task SaveAsync()
        => await context.SaveChangesAsync();


}