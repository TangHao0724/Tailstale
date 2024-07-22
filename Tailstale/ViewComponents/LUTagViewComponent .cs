using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Tailstale.Models;

namespace Tailstale.ViewComponents
{
    public class LUTagViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public LUTagViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? loginType = HttpContext.Session.GetInt32("loginType");
            int? loginId = HttpContext.Session.GetInt32("loginID");
            string? userName = null;

            if (loginType != null && loginId != null)
            {
                if (loginType == 0)
                {
                    userName = _context.keepers
                        .Where(k => k.ID == loginId)
                        .Select(k => k.name)
                        .FirstOrDefault();
                }
                else
                {
                    userName = _context.businesses
                        .Where(k => k.ID == loginId)
                        .Select(k => k.name)
                        .FirstOrDefault();
                }
            }

            ViewBag.userID = loginId;
            ViewBag.userType = loginType;
            ViewBag.userName = userName;

            return View("_LUTag");
        }
    }
}
