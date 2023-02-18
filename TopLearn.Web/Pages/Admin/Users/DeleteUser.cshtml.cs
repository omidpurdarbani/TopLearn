using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class DeleteUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public DeleteUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        private static int _userId;

        public DeleteUserViewModel DeleteUserViewModel { get; set; }

        public IActionResult OnGet(int userId = 0)
        {
            if (userId == 0)
            {
                return Redirect("/admin/users/");
            }

            if (_userService.IsUserExistByUserId(userId))
            {
                DeleteUserViewModel = _userService.GetUserInfoForDeleteUser(userId);
            }
            else
            {
                return Redirect("/admin/users/");
            }

            _userId = userId;

            DeleteUserViewModel.Roles = _permissionService.GetRoles();
            return Page();
        }

        public IActionResult OnPost()
        {
            _userService.DeleteUserFromAdmin(_userId);

            return Redirect("/Admin/Users");
        }
    }
}
