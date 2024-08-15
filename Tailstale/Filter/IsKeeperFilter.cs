using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tailstale.Filter
{
    public class IsKeeperFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            {
                /*
                    檢查是否為Keeper，若非則會挑轉到首頁
                 */

                // 獲取當前的控制器和動作名稱
                string controllerName = context.RouteData.Values["controller"].ToString();
                string actionName = context.RouteData.Values["action"].ToString();
                int? userType = context.HttpContext.Session.GetInt32("loginType");

                //如果當前路由是登入頁面，則跳過檢查
                //if (controllername == "login" && actionname == "login")
                //{
                //    return;
                //}
                //檢查登入
                if (userType == 0)
                {
                    //導向未登入的API，更換未登入內容的API。
                    return;
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }

            }

        }
    }
}
