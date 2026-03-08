using StudentRegistrationSystem.Models;
using StudentRegistrationSystem.Repositories;

namespace StudentRegistrationSystem.Services;

public class StudentService(IStudentRepository repository) : IStudentService
{
    public async Task<IEnumerable<Student>> GetAllAsync()
        => await repository.GetAllAsync();

    public async Task<Student?> GetByIdAsync(Guid id)
        => await repository.GetByIdAsync(id);

    public async Task CreateAsync(Student student)
    {
        student.Id = Guid.NewGuid();
        await repository.AddAsync(student);
        await repository.SaveAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        student.UpdatedAt = DateTime.UtcNow;
        repository.Update(student);
        await repository.SaveAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await repository.GetByIdAsync(id)
                      ?? throw new Exception("Student not found");

        await repository.SoftDeleteAsync(student);
        await repository.SaveAsync();
    }



}