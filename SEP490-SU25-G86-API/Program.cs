using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Helpers;
using SEP490_SU25_G86_API.vn.edu.fpt.Middleware;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.EmploymentTypeRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ExperienceLevelRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobLevelRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPositionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PermissionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.RolePermissionRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminAccoutServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminDashboardServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BlockedCompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.EmploymentTypeService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ExperienceLevelService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobLevelService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPositionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.PermissionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ProvinceServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.RolePermissionService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SynonymService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;
using System.Reflection;
using System.Security.Claims;
using System.Text;


namespace SEP490_SU25_G86_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
            });

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


            // Đăng ký AutoMapper
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            // Đăng ký UserDetailOfAdmin
			builder.Services.AddScoped<IUserDetailOfAdminService, UserDetailOfAdminService>();
			builder.Services.AddScoped<IUserDetailOfAdminRepository, UserDetailOfAdminRepository>();
            // Synonym
            builder.Services.AddScoped<ISynonymService, SynonymService>();
            //Company
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



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
