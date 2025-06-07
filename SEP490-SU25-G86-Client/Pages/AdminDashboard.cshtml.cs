using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "ADMIN")
            {
                // Nếu không phải admin, chuyển về trang 404
                return RedirectToPage("/NotFound");
            }
            return Page();
        }
    }
} 