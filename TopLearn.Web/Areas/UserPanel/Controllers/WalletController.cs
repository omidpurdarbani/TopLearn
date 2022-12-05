using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{

    [Area("UserPanel")]
    [Authorize]

    public class WalletController : Controller
    {

        private IUserService _userService;

        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("/wallet")]
        public IActionResult Index()
        {
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            ViewBag.ListWallet = _userService.GetUserWallet(userEmail);
            ViewBag.Balance = _userService.UserWalletBalance(userEmail);

            return View();
        }

        [Route("/wallet")]
        [HttpPost]
        public IActionResult Index(WalletChargeViewModel charge)
        {

            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!ModelState.IsValid)
            {

                ViewBag.ListWallet = _userService.GetUserWallet(userEmail);
                ViewBag.Balance = _userService.UserWalletBalance(userEmail);

                return View(charge);
            }

            //ToDo: Online payment
            {
                _userService.ChargeWallet(userEmail, charge.Amount, "شارژ حساب", true);
            }

            ViewBag.ListWallet = _userService.GetUserWallet(userEmail);
            ViewBag.Balance = _userService.UserWalletBalance(userEmail);

            return View(charge);
        }
    }
}
