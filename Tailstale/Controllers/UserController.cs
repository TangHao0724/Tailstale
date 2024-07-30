using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Tailstale.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
