using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    public class HomeController : Controller
    {
        [Area("UserPanel")]
        [Authorize]
        public IActionResult Index() => View();
    }
}
