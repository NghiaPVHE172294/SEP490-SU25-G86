using Microsoft.EntityFrameworkCore;

namespace SEP490_SU25_G86_API.Models
{
    public partial class SEP490_G86_CvMatchContext
    {
        // Method này đã được gọi từ OnModelCreating() trong file DbContext gốc
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // Chỉ lấy JobPost chưa xóa mềm
            modelBuilder.Entity<JobPost>()
                        .HasQueryFilter(j => !j.IsDelete);
            modelBuilder.Entity<JobCriterion>()
                        .HasQueryFilter(j => !j.IsDelete);
        }
    }
}

