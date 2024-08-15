using Microsoft.AspNetCore.Mvc;

namespace Tailstale.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var I = await Task.Run(() => HttpContext.Session.GetInt32("loginID"));
            var T = await Task.Run(() => HttpContext.Session.GetInt32("loginType"));
            //List<int?> ints = new List<int?> { I ,T};
            ViewBag.loginID = I;
            ViewBag.loginType = T;

            return View("_NavBar");
        }
    }
}
