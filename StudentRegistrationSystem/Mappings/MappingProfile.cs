using AutoMapper;
using StudentRegistrationSystem.DTOs;
using StudentRegistrationSystem.Models;

namespace StudentRegistrationSystem.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>().ReverseMap();
    }
}