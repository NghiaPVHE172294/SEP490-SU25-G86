using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Helpers;
using SEP490_SU25_G86_API.vn.edu.fpt.Middleware;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BanUserRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CareerHandbookRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVParsedDataRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.EmploymentTypeRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ExperienceLevelRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.HandbookCategoryRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobLevelRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPositionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.NotificationRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PermissionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PhoneOtpRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RemindRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RolePermissionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SalaryRangeRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminAccoutServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminDashboardServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BlockedCompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CareerHandbookService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.EmploymentTypeService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ExperienceLevelService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.HandbookCategoryService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobLevelService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPositionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.PhoneOtpService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ProvinceServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.RemindService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.RolePermissionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SalaryRangeService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SynonymService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService;
using SEP490_SU25_G86_API.vn.edu.fpt.SignalRHub.NotificationSignalRHub;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Twilio;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobCriterionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobCriterionService;
using vn.edu.fpt.Services.CvTemplateUpload;

namespace SEP490_SU25_G86_API
{
    public class Program
	{
		public static void Main(string[] args)
		{
			// Ưu tiên load file cấu hình mẫu nếu tồn tại (dùng cho teamwork)
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.example.json.example", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = configBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddConfiguration(configuration);
            // cấu hình Twilio
            // 1. Bind cấu hình từ appsettings.json
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

            // 2. Lấy AuthToken từ biến môi trường
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            // 3. Khởi tạo Twilio Client
            TwilioClient.Init(
                builder.Configuration["Twilio:AccountSid"],
                authToken
            );

            // Add services to the container.
            builder.Services.AddControllers();

			// Swagger with JWT Bearer support
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "bearer"
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,
						},
						new List<string>()
					}
				});
			});

			// DbContext
			builder.Services.AddDbContext<SEP490_G86_CvMatchContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// JWT Authentication
			var jwtSettings = builder.Configuration.GetSection("Jwt");
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = jwtSettings["Issuer"],
					ValidAudience = jwtSettings["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role

                };
                // >>> Thêm cấu hình cho SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // token từ query khi nâng cấp WS
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken)
                            && path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            // Dependency Injection
            // Account
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            // JobPost
            builder.Services.AddScoped<IJobPostRepository, JobPostRepository>();
            builder.Services.AddScoped<IJobPostService, JobPostService>();

            // SavedJob
            builder.Services.AddScoped<ISavedJobRepository, SavedJobRepository>();
            builder.Services.AddScoped<ISavedJobService, SavedJobService>();

            //CVsubmittion
            builder.Services.AddScoped<ICvSubmissionRepository, CvSubmissionRepository>();
            builder.Services.AddScoped<ICvSubmissionService, CvSubmissionService>();

            // AccountList
            builder.Services.AddScoped<IAccountListRepository, AccountListRepository>();
            builder.Services.AddScoped<IAccountListService, AccountListService>();

            // Province
            builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
            builder.Services.AddScoped<IProvinceService, ProvinceService>();

            // Industry
            builder.Services.AddScoped<IIndustryRepository, IndustryRepository>();
            builder.Services.AddScoped<IIndustryService, IndustryService>();

            // AdminDashboard
            builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
            builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

            // AdminSendRemind
            builder.Services.AddScoped<IRemindService, RemindService>();
            builder.Services.AddScoped<IRemindRepository, RemindRepository>();

            //GetUserByAccountIdFrAdmin
            builder.Services.AddScoped<IUserDetailOfAdminRepository, UserDetailOfAdminRepository>();
            builder.Services.AddScoped<IUserDetailOfAdminService, UserDetailOfAdminService>();

            // Permission
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();

            // RolePermission
            builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

            // JobPosition
            builder.Services.AddScoped<IJobPositionRepository, JobPositionRepository>();
            builder.Services.AddScoped<IJobPositionService, JobPositionService>();

            // AppliedJob
            builder.Services.AddScoped<IAppliedJobRepository, AppliedJobRepository>();
            builder.Services.AddScoped<IAppliedJobService, AppliedJobService>();

            // CompanyFollowing
            builder.Services.AddScoped<ICompanyFollowingRepository, CompanyFollowingRepository>();
            builder.Services.AddScoped<ICompanyFollowingService, CompanyFollowingService>();

            // AddCompany
            builder.Services.AddScoped<IAddCompanyRepository, AddCompanyRepository>();
            builder.Services.AddScoped<IAddCompanyService, AddCompanyService>();

            //BlockedCompany
            builder.Services.AddScoped<IBlockedCompanyRepository, BlockedCompanyRepository>();
            builder.Services.AddScoped<IBlockedCompanyService, BlockedCompanyService>();

            // JobLevel
            builder.Services.AddScoped<IJobLevelRepository, JobLevelRepository>();
            builder.Services.AddScoped<IJobLevelService, JobLevelService>();

            // ExperienceLevel
            builder.Services.AddScoped<IExperienceLevelRepository, ExperienceLevelRepository>();
            builder.Services.AddScoped<IExperienceLevelService, ExperienceLevelService>();

            // EmploymentType
            builder.Services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
            builder.Services.AddScoped<IEmploymentTypeService, EmploymentTypeService>();


            // SalaryRange
            builder.Services.AddScoped<ISalaryRangeRepository, SalaryRangeRepository>();
            builder.Services.AddScoped<ISalaryRangeService, SalaryRangeService>();

            //Company
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

            //User
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            //BanUser
            builder.Services.AddScoped<IBanUserService, BanUserService>();
            builder.Services.AddScoped<IBanUserRepository, BanUserRepository>();

            //InfoCompany
            builder.Services.AddScoped<IInfoCompanyService, InfoCompanyService>();
            builder.Services.AddScoped<IInfoCompanyRepository, InfoCompanyRepository>();

            // Phone OTP
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<ICacheService, CacheService>();
            builder.Services.AddScoped<IOTPProvider, TwilioOtpProvider>();
            builder.Services.AddScoped<IPhoneOtpService, PhoneOtpService>();
            builder.Services.AddScoped<IPhoneOtpRepository, PhoneOtpRepository>();

            // CareerHandbook
            builder.Services.AddScoped<ICareerHandbookRepository, CareerHandbookRepository>();
            builder.Services.AddScoped<ICareerHandbookService, CareerHandbookService>();

            // HandbookCategory
            builder.Services.AddScoped<IHandbookCategoryRepository, HandbookCategoryRepository>();
            builder.Services.AddScoped<IHandbookCategoryService, HandbookCategoryService>();    

            // JobCriterion
            builder.Services.AddScoped<IJobCriterionRepository, JobCriterionRepository>();
            builder.Services.AddScoped<IJobCriterionService, JobCriterionService>();

            //Notification
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            // Cv
            builder.Services.AddScoped<ICvRepository, CvRepository>();
            builder.Services.AddScoped<ICvService, CvService>();
            // DI-parsedCV
            builder.Services.AddScoped<IFileTextExtractor, FileTextExtractor>();
            builder.Services.AddScoped<PdfTextExtractor>();
            builder.Services.AddScoped<DocxTextExtractor>();
            builder.Services.AddScoped<IGeminiClient, GeminiClient>();
            builder.Services.AddScoped<ICVParsedDataRepository, CVParsedDataRepository>();
            builder.Services.AddScoped<ICvParsingService, CvParsingService>();

            //signalR
            builder.Services.AddSignalR();

            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            //Search Synonym
            builder.Services.AddScoped<ISynonymService, SynonymService>();
            // CORS
            builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(policy =>
				{
					policy
                          .SetIsOriginAllowed(_ => true) // cho phép mọi origin
                          .AllowAnyHeader()
						  .AllowAnyMethod()
                          .AllowCredentials();
                });
			});

            // Đăng ký dịch vụ lưu trữ Firebase cho upload CV Template
builder.Services.AddScoped<Services.CvTemplateService.IFirebaseStorageService, Services.CvTemplateService.FirebaseStorageService>();
            // Đăng ký dịch vụ upload CVTemplate cho admin
            builder.Services.AddScoped<ICvTemplateUploadService, CvTemplateUploadService>();

            // Named HttpClient độc lập cho Gemini "Gemini2"
            builder.Services.AddHttpClient("Gemini2", c =>
            {
                var baseUrl = builder.Configuration["Gemini2:Endpoint"];
                c.BaseAddress = new Uri(baseUrl);
                // Không set API key ở header vì Gemini nhận qua query ?key=
            });


            // Gemini AI Matching
            IServiceCollection serviceCollection = builder.Services.AddScoped<Services.GeminiCvJobMatchingService.IGeminiCvJobMatchingService, Services.GeminiCvJobMatchingService.GeminiCvJobMatchingService>();
            builder.Services.AddHostedService<JobExpiryWorker>();

            var app = builder.Build();

            app.UseCors();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			
			app.UseAuthentication();
            app.UseMiddleware<PermissionMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            //Map SignalR hub cho Notifications
            app.MapHub<NotificationHub>("/hubs/notifications").RequireAuthorization();

            app.Lifetime.ApplicationStarted.Register(async () =>
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<SEP490_G86_CvMatchContext>();
                var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<EndpointDataSource>>();
                var seeder = new PermissionSeeder(context, endpoints);
                await seeder.SeedPermissionsAsync();
            });
            app.Run();
		}
	}
}
