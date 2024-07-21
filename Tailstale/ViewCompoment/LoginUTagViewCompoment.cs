using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Tailstale.ViewCompoment
{
    public class LoginUTagViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int id,int Utype)
        {
            //分辨輸入ID。Usertype
            // 根據 ID 執行你的邏輯，例如從資料庫中獲取資訊
            // 傳遞資訊到 View
            //switch (Utype) { 
            
            //    case 1://旅館
            //        return View();
            //    case 2://美容
            //        return View();
            //    case 3://醫院
            //        return View();
            //    default:
            //        return View("_LoginUTagKeeper", id);
            //}
            return View("_LoginUTagKeeper", id);
            return RedirectToActionResult()

        }
    }
}
