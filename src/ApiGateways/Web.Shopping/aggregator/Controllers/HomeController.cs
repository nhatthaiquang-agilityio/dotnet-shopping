using Microsoft.AspNetCore.Mvc;

namespace Web.Shopping.HttpAggregator.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult("~/");
        }
    }
}
