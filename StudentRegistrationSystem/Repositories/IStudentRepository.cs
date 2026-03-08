using StudentRegistrationSystem.Models;

namespace StudentRegistrationSystem.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task AddAsync(Student student);
    void Update(Student student);
    Task SoftDeleteAsync(Student student);
    Task SaveAsync();
}