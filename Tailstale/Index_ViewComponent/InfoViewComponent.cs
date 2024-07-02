using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Tailstale.Models;

namespace Tailstale.Index_ViewComponent
{
    [Microsoft.AspNetCore.Mvc.ViewComponent]
    public class InfoViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var selectid = id;
            ViewData["selectedID"] = id;
            return View("Default", id);

        }
    }
}
