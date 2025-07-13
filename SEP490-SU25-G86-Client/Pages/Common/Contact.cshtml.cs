using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages
{
    public class ContactModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public bool Submitted { get; set; } = false;

        public void OnGet() { }

        public void OnPost()
        {
            // TODO: You can save to database or send an email here
            Submitted = true;
        }
    }
}
