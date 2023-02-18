using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class RestoreUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public RestoreUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        private static int _userId;

        public RestoreUserViewModel RestoreUserViewModel { get; set; }

        public IActionResult OnGet(int userId = 0)
        {
            if (userId == 0)
            {
                return Redirect("/admin/users/");
            }

            if (_userService.IsUserExistByUserId(userId))
            {
                RestoreUserViewModel = _userService.GetUserInfoForRestoreUser(userId);
            }
            else
            {
                return Redirect("/admin/users/");
            }

            _userId = userId;

            RestoreUserViewModel.Roles = _permissionService.GetRoles();
            return Page();
        }

        public IActionResult OnPost()
        {
            _userService.RestoreUserFromAdmin(_userId);

            return Redirect("/Admin/Users");
        }
    }
}
