using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {

        #region Constructor injection

        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public IActionResult Index()
        {
            return View(_userService.GetUserInformation(User.FindFirstValue(ClaimTypes.Email)));
        }

        #region Edit Profile

        [Route("/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userService.GetDataForEditUserProfile(User.FindFirstValue(ClaimTypes.Email)));
        }

        [HttpPost]
        [Route("/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.OldAvatar == true && model.UserAvatar != null)
            {
                ModelState.AddModelError("UserAvatar", "شما نمی توانید از هر دو گزینه برای عکس پروفایل استفاده نمایید !");
                return View(model);
            }

            if (model.OldAvatar == false && model.UserAvatar == null)
            {
                ModelState.AddModelError("UserAvatar", "عکسی انتخاب نشده است ! ");
                return View(model);
            }


            _userService.EditProfile(User.FindFirstValue(ClaimTypes.Email), model);

            //LogOut
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewBag.Done = true;
            return View(_userService.GetDataForEditUserProfile(User.FindFirstValue(ClaimTypes.Email)));
        }

        #endregion

        #region Change Password

        [Route("/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Route("/ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string UserEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!_userService.CompareCurrentPassword(UserEmail, model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "کلمه عبور فعلی صحیح نمی باشد");
                return View(model);
            }

            _userService.ChangeUserPassword(UserEmail, model.NewPassword);

            ViewBag.Done = true;
            ViewBag.UserName = _userService
                .GetUserByEmail(UserEmail).UserName;

            #region Logout

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            #endregion

            return View();
        }

        #endregion




    }
}
