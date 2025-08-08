namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO
{
    public class HandbookCategoryCreateDTO
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HandbookCategoryUpdateDTO
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class HandbookCategoryDetailDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class HandbookCategorySimpleDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
