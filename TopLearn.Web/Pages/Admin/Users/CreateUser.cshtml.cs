using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;

        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public CreateUserViewModel CreateUserViewModel { get; set; }

        public void OnGet()
        {
            CreateUserViewModel = new CreateUserViewModel();
            CreateUserViewModel.Roles = _permissionService.GetRoles();
        }

        public IActionResult OnPost(int selectedRole)
        {
            if (!ModelState.IsValid)
            {

                CreateUserViewModel.Roles = _permissionService.GetRoles();
                CreateUserViewModel.SelectedRole = selectedRole;

                return Page();

            }

            if (CreateUserViewModel.OldAvatar && CreateUserViewModel.UserAvatar != null)
            {

                ModelState.AddModelError("CreateUserViewModel.UserAvatar", "شما نمی توانید از هر دو گزینه برای عکس پروفایل استفاده نمایید !");

                CreateUserViewModel.Roles = _permissionService.GetRoles();
                CreateUserViewModel.SelectedRole = selectedRole;

                return Page();

            }

            if (CreateUserViewModel.OldAvatar == false && CreateUserViewModel.UserAvatar == null)
            {

                ModelState.AddModelError("CreateUserViewModel.UserAvatar", "عکسی انتخاب نشده است ! ");

                CreateUserViewModel.Roles = _permissionService.GetRoles();
                CreateUserViewModel.SelectedRole = selectedRole;

                return Page();

            }

            if (_userService.IsExistEmail(CreateUserViewModel.Email.FixEmail()))
            {
                ModelState.AddModelError("CreateUserViewModel.Email", " ایمیل قبلا ثبت شده می باشد ! ");

                CreateUserViewModel.Roles = _permissionService.GetRoles();
                CreateUserViewModel.SelectedRole = selectedRole;

                return Page();
            }

            CreateUserViewModel.SelectedRole = selectedRole;

            _userService.AddUserFromAdmin(CreateUserViewModel);

            return Redirect("/Admin/Users");
        }
    }
}
