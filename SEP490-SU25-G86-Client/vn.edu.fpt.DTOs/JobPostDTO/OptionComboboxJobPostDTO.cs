namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO
{
    public class OptionComboboxJobPostDTO
    {
        public class EmploymentTypeDTO
        {
            public int EmploymentTypeId { get; set; }
            public string EmploymentTypeName { get; set; }
        }

        public class JobPositionDTO
        {
            public int PositionId { get; set; }
            public string PostitionName { get; set; }
        }

        public class ProvinceDTO
        {
            public int ProvinceId { get; set; }
            public string ProvinceName { get; set; }
        }

        public class ExperienceLevelDTO
        {
            public int ExperienceLevelId { get; set; }
            public string ExperienceLevelName { get; set; }
        }

        public class JobLevelDTO
        {
            public int JobLevelId { get; set; }
            public string JobLevelName { get; set; }
        }

        public class IndustryDTO
        {
            public int IndustryId { get; set; }
            public string IndustryName { get; set; }
        }

        public class SalaryRangeDTO
        {
            public int SalaryRangeId { get; set; }
            public int? MinSalary { get; set; }
            public int? MaxSalary { get; set; }
            public string Currency { get; set; }
        }
    }
}
