using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EmployerDashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("user_role");
        if (role != "EMPLOYER")
        {
            return RedirectToPage("/NotFound");
        }
        return Page();
    }
} 