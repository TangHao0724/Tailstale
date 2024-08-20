using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;
using Tailstale.Tools;

namespace Tailstale.Controllers
{
    public class ECPay : Controller
    {

        //串接金流需要的資料 開始
        //將資料轉成form 再轉成文字送到前端
        [HttpPost]
        public async Task<string> GotoECPay([FromBody]SendToECPayValue myvalue)
        {
            var baseAddress = "https://localhost:7112/";
            string MerchantID = "3002599";
            string MerchantTradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            string MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string PaymentType = "aio";
            int TotalAmount = myvalue.TotalAmount;//v
            string TradeDesc = DateTime.Now.ToString("yyyy/MM/dd") + myvalue.TradeDesc;//v
            string ItemName = myvalue.ItemName;//v
            string ReturnURL = $"{baseAddress}ECPay/GetECReturnValue";
            string OrderResultURL = $"{baseAddress}Hotels/GetECOrderReturnValue";
            string ChoosePayment = "Credit";
            int EncryptType = 1;
            string CustomField1 = myvalue.BookingID;//v存hotelID
           // string CustomField2 = HttpContext.Session.GetString("GetSelectedRoom");
            //string CustomField3 = HttpContext.Session.GetString("CDToECPay"); 
            //string CustomField4 = HttpContext.Session.GetInt32("KeeperID").ToString(); 
          var order = new Dictionary<string, object>
            {
                //特店交易編號
                { "MerchantTradeNo",  MerchantTradeNo},
                
                //特店交易時間 yyyy/MM/dd HH:mm:ss
                { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},

                //交易金額
                { "TotalAmount",  TotalAmount},

                //交易描述
                { "TradeDesc", TradeDesc},

                //商品名稱
                { "ItemName", ItemName},

             
                //完成後發通知
                { "ReturnURL",  ReturnURL},

                //付款完成後導頁
                { "OrderResultURL", OrderResultURL},

               
                //特店編號， 2000132 測試綠界編號
                { "MerchantID",  "3002599"},

                //忽略付款方式
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},

                //交易類型 固定填入 aio
                { "PaymentType",  "aio"},

                //選擇預設付款方式 固定填入 ALL
                { "ChoosePayment",  "Credit"},

                {"CustomField1",CustomField1 },
                // {"CustomField1",CustomField2 },
                //  {"CustomField1",CustomField3 },
                // {"CustomField1",CustomField4},

                //CheckMacValue 加密類型 固定填入 1 (SHA256)
                { "EncryptType",  "1"},
            };

            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue1(order);

            StringBuilder s = new StringBuilder();
            s.AppendFormat("<form id='payForm' action='{0}' method='post'>", "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5");
            foreach (KeyValuePair<string, object> item in order)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", item.Key, item.Value);
            }

            s.Append("</form>");

            return s.ToString();

        }

        public string GetCheckMacValue1(Dictionary<string, object> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "spPjZn66i0OhqJsQ";

            //測試用的 HashIV
            var HashIV = "hT5OJckN45isQTTs";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = EncryptSHA256(checkValue);

            return checkValue.ToUpper();
        }

        public string EncryptSHA256(string source)
        {
            string result = string.Empty;

            using (System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }
            return result;
        }

        public Dictionary<string, object> GetECReturnValue(IFormCollection returnform)
        {
            //   PaymentDate
            //  PaymentType
            //RtnCode
            //RtnMsg
            //"[CustomField1, ]"

            var a = returnform;
            Dictionary<string, object> returnDict = new Dictionary<string, object>();
            foreach (var item in a)
            {
                returnDict[item.Key] = item.Value;
            }

            return returnDict;
        }
        public Dictionary<string, object> GetECOrderReturnValue(IFormCollection returnform)
        {
            //   PaymentDate
            //  PaymentType
            //RtnCode
            //RtnMsg
            //"[CustomField1, ]"

            var a = returnform;
            Dictionary<string, object> returnDict = new Dictionary<string, object>();
            foreach (var item in a)
            {
                returnDict[item.Key] = item.Value;
            }

            return returnDict;
        }
        //串接金流需要的資料 結束
    }
}
