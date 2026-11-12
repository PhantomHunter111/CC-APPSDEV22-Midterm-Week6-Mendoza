using AutoMapper;
using StudentRosterApi.DTOs;
using StudentRosterApi.Models;

namespace StudentRosterApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src =>
                    src.Department == null ? "Unassigned" : src.Department.Name))
            .ForMember(dest => dest.CourseTitles,
                opt => opt.MapFrom(src => src.Courses.Select(course => course.Title).ToList()));

        CreateMap<StudentCreateDto, Student>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Courses, opt => opt.Ignore());
    }
}
