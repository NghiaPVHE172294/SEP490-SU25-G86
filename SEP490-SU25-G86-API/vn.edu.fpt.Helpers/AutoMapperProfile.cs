using AutoMapper;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.EmploymentTypeDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ExperienceLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPositionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ProvinceDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;

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

            CreateMap<JobPost, ViewDetailJobPostDTO>()
            .ForMember(dest => dest.EmployerName,
                       opt => opt.MapFrom(src => src.Employer.FullName))
            .ForMember(dest => dest.IndustryName,
                       opt => opt.MapFrom(src => src.Industry.IndustryName))
            .ForMember(dest => dest.JobPositionName,
                       opt => opt.MapFrom(src => src.JobPosition.PostitionName))
            .ForMember(dest => dest.SalaryRangeName,
                       opt => opt.MapFrom(src => src.SalaryRange != null
                           ? $"{src.SalaryRange.MinSalary:N0} - {src.SalaryRange.MaxSalary:N0} {src.SalaryRange.Currency}"
                           : null))
            .ForMember(dest => dest.ProvinceName,
                       opt => opt.MapFrom(src => src.Province.ProvinceName))
            .ForMember(dest => dest.ExperienceLevelName,
                       opt => opt.MapFrom(src => src.ExperienceLevel.ExperienceLevelName))
            .ForMember(dest => dest.JobLevelName,
                       opt => opt.MapFrom(src => src.JobLevel.JobLevelName))
            .ForMember(dest => dest.EmploymentTypeName,
                       opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
            .ForMember(dest => dest.CompanyName,
                       opt => opt.MapFrom(src => src.Employer.Company.CompanyName
                           ?? src.Employer.FullName
                           ?? "Không rõ"))
            .ForMember(dest => dest.LogoUrl,
                       opt => opt.MapFrom(src => src.Employer.Company.LogoUrl)) // ✅ Map LogoUrl
            .ForMember(dest => dest.CompanySize,
                       opt => opt.MapFrom(src => src.Employer.Company.CompanySize)) 
            .ForMember(dest => dest.Address,
                       opt => opt.MapFrom(src => src.Employer.Company.Address)) 
            .ForMember(dest => dest.Website,
                       opt => opt.MapFrom(src => src.Employer.Company.Website)) 

            .ForMember(dest => dest.CvTemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.CvTemplateName, opt => opt.Ignore())
            .ForMember(dest => dest.DocFileUrl, opt => opt.Ignore())
            .ForMember(dest => dest.PdfFileUrl, opt => opt.Ignore());

            CreateMap<JobPost, JobPostListDTO>()
    .ForMember(dest => dest.JobPostId, opt => opt.MapFrom(src => src.JobPostId))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
    .ForMember(dest => dest.CompanyName,
               opt => opt.MapFrom(src => src.Employer.Company.CompanyName
                   ?? src.Employer.FullName
                   ?? "Không rõ"))
    .ForMember(dest => dest.Salary,
               opt => opt.MapFrom(src => src.SalaryRange != null &&
                                          src.SalaryRange.MinSalary.HasValue &&
                                          src.SalaryRange.MaxSalary.HasValue
                   ? $"{src.SalaryRange.MinSalary:N0} - {src.SalaryRange.MaxSalary:N0} {src.SalaryRange.Currency}"
                   : "Thỏa thuận"))
    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Province.ProvinceName))
    .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
    .ForMember(dest => dest.JobLevel, opt => opt.MapFrom(src => src.JobLevel.JobLevelName))
    .ForMember(dest => dest.ExperienceLevel, opt => opt.MapFrom(src => src.ExperienceLevel.ExperienceLevelName))
    .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Industry.IndustryName))
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
    .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
    .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
    .ForMember(dest => dest.WorkLocation, opt => opt.MapFrom(src => src.WorkLocation));

            CreateMap<JobPost, RelatedJobItemDTO>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Employer.Company.CompanyName))
                .ForMember(d => d.ProvinceName, o => o.MapFrom(s => s.Province.ProvinceName))
                .ForMember(d => d.MinSalary, o => o.MapFrom(s => s.SalaryRange.MinSalary))
                .ForMember(d => d.MaxSalary, o => o.MapFrom(s => s.SalaryRange.MaxSalary))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.SalaryRange.Currency));
        }
    }
} 