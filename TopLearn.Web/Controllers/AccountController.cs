using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {

        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService; 
        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_userService.IsExistUserName(model.UserName))
            {
                ModelState.AddModelError("UserName","نام کاربری معتبر نمی باشد");
                return View(model);
            }

            if (_userService.IsExistEmail(model.Email.FixEmail()))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(model);
            }

            //ToDo: Register User

            return View();
        }
    }
}
