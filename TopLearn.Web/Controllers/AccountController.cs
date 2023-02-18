using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Senders;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{

    public class AccountController : Controller
    {

        #region Constructor injection

        private static bool sended = false;
        private static User _user = new User();

        private IUserService _userService;
        private IViewRenderService _viewRenderService;

        public AccountController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
        }

        #endregion

        #region Register

        [Route("/Register")]
        public IActionResult Register()
        {
            if (sended == true)
            {
                ViewBag.UserName = _user.UserName;
                ViewBag.Email = _user.Email;
                ViewBag.Resend = true;

                sended = false;
                _user = null;

                return View();
            }

            return View();
        }

        [Route("/Register")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
                UserAvatar = "Default.jpg",
                UserName = model.UserName,
                UserRole = 3
            };

            _userService.AddUser(user);

            //Send Email
            SendActiveEmail(user.Email);

            ViewBag.UserName = _user.UserName;
            ViewBag.Email = _user.Email;
            ViewBag.Done = true;

            sended = false;
            _user = null;

            return View(model);

        }

        [Route("/SendActiveEmail/{email}")]
        public IActionResult SendActiveEmail(string email)
        {

            if (email == null)
            {
                return View("Register");
            }

            var user = _userService.GetUserByEmail(email);

            if (user == null)
            {
                return Redirect("/Register");
            }

            _userService.UpdateUser(user);

            //send email
            string body = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "فعال سازی حساب", body);

            _user = user;
            sended = true;
            return RedirectToAction("Register");
        }

        #endregion

        #region Login

        [Route("/Login")]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();

            string returnUrl = HttpContext.Request.Query["ReturnUrl"];
            if (returnUrl != null)
            {
                model.ReturnPath = returnUrl;
            }

            return View(model);
        }

        [Route("/Login")]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userService.LoginUser(model.Email, model.Password);

            if (user != null)
            {
                if (user.IsActive == false)
                {
                    ViewBag.UserName = user.UserName;
                    ViewBag.Email = user.Email;
                    ViewBag.notactive = true;
                    return View();
                }
                //Login User
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var properties = new AuthenticationProperties()
                {
                    IsPersistent = model.RememberMe
                };

                HttpContext.SignInAsync(principal, properties);

                if (model.ReturnPath != null)
                {
                    return Redirect(model.ReturnPath);
                }

                ViewBag.UserName = user.UserName;
                ViewBag.Done = true;
                return View(model);

            }

            ModelState.AddModelError("Email", "اطلاعات وارد شده معتبر نمی باشند !");

            return View(model);
        }

        #endregion

        #region Logout

        [Route("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        #endregion

        #region Active account

        public IActionResult ActiveAccount(string id)
        {
            User user = _userService.ActiveAccount(id);

            if (user != null)
            {

                return View("ActiveAccount", user.UserName);

            }

            return NotFound();

        }

        #endregion

        #region Forgot password

        [Route("/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("/ForgotPassword")]
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = _userService.GetUserByEmail(model.Email.FixEmail());

            if (user == null)
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد!");
                return View(model);
            }

            #region Send Recovery Email

            string body = _viewRenderService.RenderToStringAsync("_ForgotPassword", user);
            SendEmail.Send(user.Email, "بازیابی کلمه عبور", body);

            #endregion

            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.Done = true;
            return View(model);
        }
        #endregion

        #region Reset password

        public IActionResult ResetPassword(string id)
        {
            User user = _userService.GetUserByActiveCode(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(new ResetPasswordViewModel()
            {
                ActiveCode = id
            });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = _userService.GetUserByActiveCode(model.ActiveCode);

            if (user == null)
            {
                return NotFound();
            }

            string hashNewPassword = PasswordHelper.EncodePasswordMd5(model.Password);
            user.Password = hashNewPassword;
            _userService.UpdateUser(user);

            #region Logout

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            #endregion

            ViewBag.Done = true;
            return View(model);
        }
        #endregion

    }
}
