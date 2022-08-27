using System;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {

        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_userService.IsExistUserName(model.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری معتبر نمی باشد");
                return View(model);
            }

            if (_userService.IsExistEmail(model.Email.FixEmail()))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(model);
            }

            User user = new User()
            {
                ActiveCode = TextGenerator.GenerateUniqCode(),
                Email = model.Email.FixEmail(),
                IsActive = false,
                Password = PasswordHelper.EncodePasswordMd5(model.Password),
                RegisterDate = DateTime.Now,
                UserAvatar = "Defult.jpg",
                UserName = model.UserName
            };

            _userService.AddUser(user);

            return View("SuccessRegister", user);

        }

        #endregion

        #region Login

        

        #endregion

    }
}
