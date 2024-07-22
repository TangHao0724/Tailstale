using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_COREMVC
{
    public class IsLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            {
                /*
                    檢查是否登入，若未登入則會挑轉到首頁
                 */

                // 獲取當前的控制器和動作名稱
                string controllerName = context.RouteData.Values["controller"].ToString();
                string actionName = context.RouteData.Values["action"].ToString();
                string userID = context.HttpContext.Session.GetString("loginID");




                //檢查登入
                //如果當前路由是登入頁面，登入狀態下會跳轉回首頁
                if (controllerName == "LNR")
                {
                    if (userID != null)
                    {
                        //導向未登入的Action，更換未登入內容的Action。
                        context.Result = new RedirectToActionResult("Index", "Home", null);
                    }
                }
                else if (userID == null)
                {
                    //導向未登入的API，更換未登入內容的API。
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                    
                }                


            }

        }
    }
}
