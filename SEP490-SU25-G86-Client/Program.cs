namespace SEP490_SU25_G86_Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });
            builder.Services.AddRazorPages();
            builder.Services.AddSession();
            builder.Services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7004/");
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.  
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            

            // Map default route to Login.cshtml  
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Common/Homepage");
                //context.Response.Redirect("/Common/ForgotPassword");
                return Task.CompletedTask;
            });

            app.MapRazorPages();

            app.Run();
        }
    }
}
