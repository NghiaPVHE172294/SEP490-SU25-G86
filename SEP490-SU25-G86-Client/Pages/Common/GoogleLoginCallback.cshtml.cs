using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class GoogleLoginCallbackModel : PageModel
    {
        public IActionResult OnGet(string token, string role, int userId)
        {
            HttpContext.Session.SetString("jwt_token", token);
            HttpContext.Session.SetString("user_role", role);
            HttpContext.Session.SetString("userId", userId.ToString());
            return RedirectToPage("/Common/Homepage");
        }
    }
} 