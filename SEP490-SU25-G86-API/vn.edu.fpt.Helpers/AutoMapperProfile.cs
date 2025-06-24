using AutoMapper;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPositionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ProvinceDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ExperienceLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.EmploymentTypeDTO;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // JobPosition
            CreateMap<AddJobPositionDTO, JobPosition>()
                .ForMember(dest => dest.PostitionName, opt => opt.MapFrom(src => src.JobPositionName));
            // Province
            CreateMap<AddProvinceDTO, Province>()
                .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.ProvinceName));
            // ExperienceLevel
            CreateMap<AddExperienceLevelDTO, ExperienceLevel>()
                .ForMember(dest => dest.ExperienceLevelName, opt => opt.MapFrom(src => src.ExperienceLevelName));
            // JobLevel
            CreateMap<AddJobLevelDTO, JobLevel>()
                .ForMember(dest => dest.JobLevelName, opt => opt.MapFrom(src => src.JobLevelName));
            // EmploymentType
            CreateMap<AddEmploymentTypeDTO, EmploymentType>()
                .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.EmploymentTypeName));
            // Industry
            CreateMap<AddIndustryDTO, Industry>()
                .ForMember(dest => dest.IndustryName, opt => opt.MapFrom(src => src.IndustryName));
        }
    }
} 