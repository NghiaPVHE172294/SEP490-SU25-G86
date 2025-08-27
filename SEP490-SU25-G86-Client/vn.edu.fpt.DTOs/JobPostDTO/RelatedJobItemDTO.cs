namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO
{
    public class RelatedJobItemDTO
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = "";

        public string? CompanyName { get; set; }
        public string? ProvinceName { get; set; }

        // Để format lương đẹp: lấy min/max/currency rồi ghép chuỗi
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string? Currency { get; set; }

        public string SalaryRangeName =>
            (MinSalary == null && MaxSalary == null)
                ? "Thỏa thuận"
                : $"{(MinSalary?.ToString("N0") ?? "?")} - {(MaxSalary?.ToString("N0") ?? "?")} {(Currency ?? "")}".Trim();
    }

}
