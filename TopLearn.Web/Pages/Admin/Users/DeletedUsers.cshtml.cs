using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class DeletedUsersModel : PageModel
    {
        private IUserService _userService;

        public DeletedUsersModel(IUserService userService)
        {
            _userService = userService;
        }

        public UsersForAdminViewModel UsersForAdminViewModel { get; set; }

        public void OnGet(int pageId = 1, string filterEmail = "", string filterUsername = "")
        {

            UsersForAdminViewModel = _userService.GetDeletedUsers(10, pageId, filterEmail, filterUsername);

            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Query["filterUsername"]))
            {
                ViewData["name"] = HttpContext.Request.Query["filterUsername"];
            }
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Query["filterEmail"]))
            {
                ViewData["email"] = HttpContext.Request.Query["filterEmail"];
            }

        }

    }
}
