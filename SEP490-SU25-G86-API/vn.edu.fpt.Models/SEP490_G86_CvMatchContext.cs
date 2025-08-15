using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SEP490_SU25_G86_API.Models
{
    public partial class SEP490_G86_CvMatchContext : DbContext
    {
        public SEP490_G86_CvMatchContext()
        {
        }

        public SEP490_G86_CvMatchContext(DbContextOptions<SEP490_G86_CvMatchContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public virtual DbSet<BlockedCompany> BlockedCompanies { get; set; } = null!;
        public virtual DbSet<CareerHandbook> CareerHandbooks { get; set; } = null!;
        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<CompanyFollower> CompanyFollowers { get; set; } = null!;
        public virtual DbSet<Cv> Cvs { get; set; } = null!;
        public virtual DbSet<CvTemplate> CvTemplates { get; set; } = null!;
        public virtual DbSet<CvTemplateForJobpost> CvTemplateForJobposts { get; set; } = null!;
        public virtual DbSet<CvTemplateOfEmployer> CvTemplateOfEmployers { get; set; } = null!;
        public virtual DbSet<Cvlabel> Cvlabels { get; set; } = null!;
        public virtual DbSet<CvparsedDatum> CvparsedData { get; set; } = null!;
        public virtual DbSet<Cvsubmission> Cvsubmissions { get; set; } = null!;
        public virtual DbSet<EmploymentType> EmploymentTypes { get; set; } = null!;
        public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; } = null!;
        public virtual DbSet<HandbookCategory> HandbookCategories { get; set; } = null!;
        public virtual DbSet<Industry> Industries { get; set; } = null!;
        public virtual DbSet<JobCriterion> JobCriteria { get; set; } = null!;
        public virtual DbSet<JobLevel> JobLevels { get; set; } = null!;
        public virtual DbSet<JobPosition> JobPositions { get; set; } = null!;
        public virtual DbSet<JobPost> JobPosts { get; set; } = null!;
        public virtual DbSet<JobPostView> JobPostViews { get; set; } = null!;
        public virtual DbSet<MatchedCvandJobPost> MatchedCvandJobPosts { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Remind> Reminds { get; set; } = null!;
        public virtual DbSet<RequireOfCompany> RequireOfCompanies { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public virtual DbSet<SalaryRange> SalaryRanges { get; set; } = null!;
        public virtual DbSet<SavedJob> SavedJobs { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=SEP490_G86_CvMatch;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.Email, "Email")
                    .IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accounts_Roles");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.Property(e => e.Action).HasMaxLength(200);

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.TargetTable).HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AuditLogs_Users");
            });

            modelBuilder.Entity<BlockedCompany>(entity =>
            {
                entity.HasKey(e => e.BlockedCompaniesId);

                entity.Property(e => e.BlockedCompaniesId).HasColumnName("BlockedCompaniesID");

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.BlockedCompanies)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockedCompanies_Users");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.BlockedCompanies)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BlockedCompanies_Companies");
            });

            modelBuilder.Entity<CareerHandbook>(entity =>
            {
                entity.HasKey(e => e.HandbookId);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Slug).HasMaxLength(200);

                entity.Property(e => e.Tags).HasMaxLength(200);

                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CareerHandbooks)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CareerHandbooks_HandbookCategories");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.CompanyName).HasMaxLength(255);

                entity.Property(e => e.CompanySize).HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.LogoUrl).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.TaxCode).HasMaxLength(50);

                entity.Property(e => e.Website).HasMaxLength(255);

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.Companies)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Companies_Industries");
            });

            modelBuilder.Entity<CompanyFollower>(entity =>
            {
                entity.HasKey(e => e.FollowId);

                entity.Property(e => e.FlowedAt).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyFollowers)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyFollowers_Companies");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CompanyFollowers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyFollowers_Users");
            });

            modelBuilder.Entity<Cv>(entity =>
            {
                entity.ToTable("CVs");

                entity.Property(e => e.Cvname)
                    .HasMaxLength(50)
                    .HasColumnName("CVName");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.SaveStatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.UploadByUser)
                    .WithMany(p => p.Cvs)
                    .HasForeignKey(d => d.UploadByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CVs_Users");
            });

            modelBuilder.Entity<CvTemplate>(entity =>
            {
                entity.Property(e => e.CvTemplateName).HasMaxLength(30);

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.CvTemplates)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplates_Users");

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.CvTemplates)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplates_Industries");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.CvTemplates)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplates_JobPositions");
            });

            modelBuilder.Entity<CvTemplateForJobpost>(entity =>
            {
                entity.ToTable("CvTemplateForJobpost");

                entity.Property(e => e.IsDisplay)
                    .IsRequired()
                    .HasColumnName("isDisplay")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CvtemplateOfEmployer)
                    .WithMany(p => p.CvTemplateForJobposts)
                    .HasForeignKey(d => d.CvtemplateOfEmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplateForJobpost_CvTemplateOfEmployer");

                entity.HasOne(d => d.JobPost)
                    .WithMany(p => p.CvTemplateForJobposts)
                    .HasForeignKey(d => d.JobPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplateForJobpost_JobPosts");
            });

            modelBuilder.Entity<CvTemplateOfEmployer>(entity =>
            {
                entity.ToTable("CvTemplateOfEmployer");

                entity.Property(e => e.CvTemplateName).HasMaxLength(50);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.UploadDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.CvTemplateOfEmployers)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CvTemplateOfEmployer_Users");
            });

            modelBuilder.Entity<Cvlabel>(entity =>
            {
                entity.HasKey(e => e.LabelId);

                entity.ToTable("CVLabels");

                entity.Property(e => e.ColorCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.LabelName).HasMaxLength(100);
            });

            modelBuilder.Entity<CvparsedDatum>(entity =>
            {
                entity.HasKey(e => e.CvparsedDataId);

                entity.ToTable("CVParsedData");

                entity.Property(e => e.CvparsedDataId).HasColumnName("CVParsedDataId");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.EducationLevel).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.FullName)
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.Languages).HasMaxLength(200);

                entity.Property(e => e.ParsedAt).HasColumnType("datetime");

                entity.Property(e => e.Phone).HasMaxLength(30);

                entity.HasOne(d => d.Cv)
                    .WithMany(p => p.CvparsedData)
                    .HasForeignKey(d => d.CvId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CVParsedData_CVs");
            });

            modelBuilder.Entity<Cvsubmission>(entity =>
            {
                entity.HasKey(e => e.SubmissionId);

                entity.ToTable("CVSubmissions");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.LabelSource).HasMaxLength(20);

                entity.Property(e => e.MatchedCvandJobPostId).HasColumnName("MatchedCVandJobPostId");

                entity.Property(e => e.SourceType).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'(Chờ xử lý)')");

                entity.Property(e => e.SubmissionDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Cv)
                    .WithMany(p => p.Cvsubmissions)
                    .HasForeignKey(d => d.CvId)
                    .HasConstraintName("FK_CVSubmissions_CVs");

                entity.HasOne(d => d.JobPost)
                    .WithMany(p => p.Cvsubmissions)
                    .HasForeignKey(d => d.JobPostId)
                    .HasConstraintName("FK_CVSubmissions_JobPosts");

                entity.HasOne(d => d.Label)
                    .WithMany(p => p.Cvsubmissions)
                    .HasForeignKey(d => d.LabelId)
                    .HasConstraintName("FK_CVSubmissions_CVLabels");

                entity.HasOne(d => d.MatchedCvandJobPost)
                    .WithMany(p => p.Cvsubmissions)
                    .HasForeignKey(d => d.MatchedCvandJobPostId)
                    .HasConstraintName("FK_CVSubmissions_MatchedCVandJobPost");

                entity.HasOne(d => d.SubmittedByUser)
                    .WithMany(p => p.Cvsubmissions)
                    .HasForeignKey(d => d.SubmittedByUserId)
                    .HasConstraintName("FK_CVSubmissions_Users");
            });

            modelBuilder.Entity<EmploymentType>(entity =>
            {
                entity.Property(e => e.EmploymentTypeName).HasMaxLength(100);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            });

            modelBuilder.Entity<ExperienceLevel>(entity =>
            {
                entity.Property(e => e.ExperienceLevelName).HasMaxLength(50);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            });

            modelBuilder.Entity<HandbookCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(300);
            });

            modelBuilder.Entity<Industry>(entity =>
            {
                entity.Property(e => e.IndustryName).HasMaxLength(100);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            });

            modelBuilder.Entity<JobCriterion>(entity =>
            {
                entity.HasKey(e => e.JobCriteriaId);

                entity.HasIndex(e => e.JobPostId, "UQ_JobCriteria_JobPost")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EducationLevel).HasMaxLength(100);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.PreferredLanguages).HasMaxLength(200);

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.JobCriteria)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobCriteria_Users");
            });

            modelBuilder.Entity<JobLevel>(entity =>
            {
                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.JobLevelName).HasMaxLength(50);
            });

            modelBuilder.Entity<JobPosition>(entity =>
            {
                entity.HasKey(e => e.PositionId);

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.PostitionName).HasMaxLength(100);

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.JobPositions)
                    .HasForeignKey(d => d.IndustryId)
                    .HasConstraintName("FK_JobPositions_Industries");
            });

            modelBuilder.Entity<JobPost>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.IsAienabled).HasColumnName("IsAIEnabled");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkLocation).HasMaxLength(255);

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.EmployerId)
                    .HasConstraintName("FK_JobPosts_Users");

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK_JobPosts_EmploymentTypes");

                entity.HasOne(d => d.ExperienceLevel)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.ExperienceLevelId)
                    .HasConstraintName("FK_JobPosts_ExperienceLevels");

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.IndustryId)
                    .HasConstraintName("FK_JobPosts_Industries");

                entity.HasOne(d => d.JobLevel)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.JobLevelId)
                    .HasConstraintName("FK_JobPosts_JobLevels");

                entity.HasOne(d => d.JobPosition)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.JobPositionId)
                    .HasConstraintName("FK_JobPosts_JobPositions");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_JobPosts_Provinces");

                entity.HasOne(d => d.SalaryRange)
                    .WithMany(p => p.JobPosts)
                    .HasForeignKey(d => d.SalaryRangeId)
                    .HasConstraintName("FK_JobPosts_SalaryRanges");
            });

            modelBuilder.Entity<JobPostView>(entity =>
            {
                entity.HasKey(e => e.ViewId);

                entity.Property(e => e.ViewAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.JobPost)
                    .WithMany(p => p.JobPostViews)
                    .HasForeignKey(d => d.JobPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobPostViews_JobPosts");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.JobPostViews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobPostViews_Users");
            });

            modelBuilder.Entity<MatchedCvandJobPost>(entity =>
            {
                entity.ToTable("MatchedCVandJobPost");

                entity.Property(e => e.MatchedCvandJobPostId).HasColumnName("MatchedCVandJobPostId");

                entity.Property(e => e.CvparsedDataId).HasColumnName("CVParsedDataId");

                entity.HasOne(d => d.CvparsedData)
                    .WithMany(p => p.MatchedCvandJobPosts)
                    .HasForeignKey(d => d.CvparsedDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchedCVandJobPost_CVParsedData");

                entity.HasOne(d => d.JobPostCriteria)
                    .WithMany(p => p.MatchedCvandJobPosts)
                    .HasForeignKey(d => d.JobPostCriteriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchedCVandJobPost_JobCriteria");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => new { e.ReceiverUserId, e.IsRead }, "IX_Notifications_IsRead");

                entity.HasIndex(e => new { e.ReceiverUserId, e.CreatedAt }, "IX_Notifications_Receiver_CreatedAt");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.ReadAt).HasColumnType("datetime");

                entity.HasOne(d => d.ReceiverUser)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ReceiverUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Receiver");
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.IsUsed).HasDefaultValueSql("((0))");

                entity.Property(e => e.Token).HasMaxLength(100);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.PasswordResetTokens)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PasswordResetTokens_Accounts");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Endpoint).HasMaxLength(200);

                entity.Property(e => e.Method).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.ProvinceName).HasMaxLength(100);

                entity.Property(e => e.Region).HasMaxLength(50);
            });

            modelBuilder.Entity<Remind>(entity =>
            {
                entity.ToTable("Remind");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FromEmail).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.ToEmail)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<RequireOfCompany>(entity =>
            {
                entity.ToTable("RequireOfCompany");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.RequireOfCompanies)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_RequireOfCompany_Companies");

                entity.HasOne(d => d.SendByUser)
                    .WithMany(p => p.RequireOfCompanies)
                    .HasForeignKey(d => d.SendByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RequireOfCompany_Users");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId })
                    .HasName("PK__RolePerm__6400A1A8C29AE4CD");

                entity.Property(e => e.IsAuthorized).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RolePermi__Permi__367C1819");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RolePermi__RoleI__37703C52");
            });

            modelBuilder.Entity<SalaryRange>(entity =>
            {
                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("(N'VND')");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            });

            modelBuilder.Entity<SavedJob>(entity =>
            {
                entity.HasKey(e => e.SaveJobId);

                entity.Property(e => e.SaveAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.JobPost)
                    .WithMany(p => p.SavedJobs)
                    .HasForeignKey(d => d.JobPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SavedJobs_JobPosts");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SavedJobs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SavedJobs_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.AccountId, "UQ_Users_Accounts")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("DOB");

                entity.Property(e => e.FullName).HasMaxLength(30);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.IsBan).HasColumnName("isBan");

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Accounts");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Users_Companies");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
