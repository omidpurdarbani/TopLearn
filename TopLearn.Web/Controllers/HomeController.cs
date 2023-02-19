using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Route("Factor/{id}")]
        public IActionResult Factor(int id)
        {
            Wallet _wallet = new Wallet();
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            _wallet = _userService.GetWalletByWalletId(id);
            if (_wallet == null)
            {
                return NotFound();
            }

            int userId = _userService.GetUserIdByEmail(User.FindFirstValue(ClaimTypes.Email));
            if (_wallet.UserId != userId)
            {
                return NotFound();
            }

            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"]
                    .ToString()
                    .ToLower().Contains("ok") &&
                HttpContext.Request.Query["Authorith"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];

                var payment = new ZarinpalSandbox.Payment(_wallet.Amount);

                var res = payment.Verification(authority).Result;

                if (res.Status == 100 || res.Status == 101)
                {
                    _userService.UpdateWalletFactorUrl(id, HttpContext.Request.GetDisplayUrl());
                    _wallet.IsPay = true;
                    _userService.UpdateWallet(_wallet);
                    ViewBag.code = res.RefId;
                    ViewBag.IsSuccess = true;
                }
            }

            ViewBag.Balance = _userService.UserWalletBalance(User.FindFirstValue(ClaimTypes.Email));
            ViewBag.id = id;
            ViewBag.date = _wallet.CreateDate.ToShortDateString();
            ViewBag.time = _wallet.CreateDate.ToShortTimeString();
            ViewBag.UserName = User.Identity.Name;
            return View();
        }
    }
}