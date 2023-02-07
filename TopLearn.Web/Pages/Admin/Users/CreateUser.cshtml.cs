using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class CreateUserModel : PageModel
    {
        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }

        public void OnGet()
        {

        }
    }
}
