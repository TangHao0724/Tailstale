using Microsoft.AspNetCore.Mvc;
using Tailstale.Models;

namespace Tailstale.ViewComponents
{
    public class RePostViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public RePostViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? loginId = HttpContext.Session.GetInt32("loginID");

            ViewBag.userID = loginId;
            return View("_Repost");
        }
    }
}
