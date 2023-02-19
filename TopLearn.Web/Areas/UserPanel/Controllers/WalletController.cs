using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopLearn.Core.DTOs.User;
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

            var Wallets = _userService.GetUserWallet(userEmail);
            Wallets.Reverse(0, Wallets.Count);
            ViewBag.ListWallet = Wallets;
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


            int walletId = _userService.ChargeWallet(userEmail, charge.Amount, "شارژ حساب", false);
            _userService.UpdateWalletFactorUrl(walletId, "https://localhost:44349/Factor/");

            #region Online Payment

            var payment = new ZarinpalSandbox.Payment(charge.Amount);
            var res = payment.PaymentRequest("شارژ حساب", "https://localhost:44349/Factor/" + walletId,
                "omidprojecttest@gmail.com");

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }

            #endregion


            ViewBag.ListWallet = _userService.GetUserWallet(userEmail);
            ViewBag.Balance = _userService.UserWalletBalance(userEmail);

            return View(charge);
        }
    }
}
