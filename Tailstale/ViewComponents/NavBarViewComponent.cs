using Microsoft.AspNetCore.Mvc;

namespace Tailstale.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.userID = await Task.Run(() => HttpContext.Session.GetInt32("loginID"));
            ViewBag.userType = await Task.Run(() => HttpContext.Session.GetInt32("loginType"));


            return View("_NavBar");
        }
    }
}
