using StudentRegistrationSystem.Models;

namespace StudentRegistrationSystem.Services;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
}