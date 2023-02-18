using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        private static int _userId;

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }

        public IActionResult OnGet(int userId = 0)
        {
            if (userId == 0)
            {
                return Redirect("/admin/users/");
            }
            if (_userService.IsUserExistByUserId(userId))
            {
                EditUserViewModel = _userService.GetUserInfoForEditUser(userId);
            }
            else
            {
                return Redirect("/admin/users/");
            }

            _userId = userId;

            EditUserViewModel.Roles = _permissionService.GetRoles();
            return Page();
        }

        public IActionResult OnPost(int selectedRole)
        {
            if (!ModelState.IsValid)
            {

                EditUserViewModel.Roles = _permissionService.GetRoles();
                EditUserViewModel.SelectedRole = selectedRole;
                EditUserViewModel.CurrentAvatar = (_userService.GetUserInfoForEditUser(_userId)).CurrentAvatar;

                return Page();

            }

            if (EditUserViewModel.OldAvatar && EditUserViewModel.UserAvatar != null)
            {

                ModelState.AddModelError("EditUserViewModel.UserAvatar", "شما نمی توانید از هر دو گزینه برای عکس پروفایل استفاده نمایید !");

                EditUserViewModel.Roles = _permissionService.GetRoles();
                EditUserViewModel.SelectedRole = selectedRole;
                EditUserViewModel.CurrentAvatar = (_userService.GetUserInfoForEditUser(_userId)).CurrentAvatar;

                return Page();

            }

            if (EditUserViewModel.OldAvatar == false && EditUserViewModel.UserAvatar == null)
            {

                ModelState.AddModelError("EditUserViewModel.UserAvatar", " عکسی انتخاب نشده است ");

                EditUserViewModel.Roles = _permissionService.GetRoles();
                EditUserViewModel.SelectedRole = selectedRole;
                EditUserViewModel.CurrentAvatar = (_userService.GetUserInfoForEditUser(_userId)).CurrentAvatar;

                return Page();

            }

            if (_userService.IsExistEmail(EditUserViewModel.Email.FixEmail()))
            {
                ModelState.AddModelError("EditUserViewModel.Email", " ایمیل قبلا ثبت شده می باشد ! ");

                EditUserViewModel.Roles = _permissionService.GetRoles();
                EditUserViewModel.SelectedRole = selectedRole;
                EditUserViewModel.CurrentAvatar = (_userService.GetUserInfoForEditUser(_userId)).CurrentAvatar;

                return Page();
            }

            EditUserViewModel.SelectedRole = selectedRole;

            _userService.EditUserFromAdmin(EditUserViewModel, _userId);

            return Redirect("/Admin/Users");
        }
    }
}
