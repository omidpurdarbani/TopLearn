using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.DTOs;
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

        [Route("/UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userService.GetDataForEditUserProfile(User.FindFirstValue(ClaimTypes.Email)));
        }

        [HttpPost]
        [Route("/UserPanel/EditProfile")]
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
                ModelState.AddModelError("UserAvatar", "عکسی انتخاب نشده است !");
                return View(model);
            }


            _userService.EditProfile(User.FindFirstValue(ClaimTypes.Email), model);

            ViewBag.Done = true;
            return View(_userService.GetDataForEditUserProfile(User.FindFirstValue(ClaimTypes.Email)));
        }

    }
}
