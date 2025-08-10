namespace SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO
{
    public class CareerHandbookCreateDTO
    {
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Tags { get; set; }
        public int CategoryId { get; set; }
        public bool IsPublished { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
    }

    public class CareerHandbookUpdateDTO
    {
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Tags { get; set; }
        public int CategoryId { get; set; }
        public bool IsPublished { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
    }

    public class CareerHandbookDetailDTO
    {
        public int HandbookId { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }
        public string? Tags { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
