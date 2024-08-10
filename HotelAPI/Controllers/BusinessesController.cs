using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Models;
using WebApplication1.DTO;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Net;
using System.Text;
using System.Web;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessesController : ControllerBase
    {
        public TailstaleContext _context;

        public BusinessesController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: api/Businesses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Business>>> GetBusinesses()
        {
            return await _context.businesses.ToListAsync();
        }

        
        // GET: api/Businesses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> GetBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);

            if (business == null)
            {
                return NotFound();
            }

            return business;
        }

        
        [HttpGet("ConvertAddressToJsonString")]
        public async Task<string> ConvertAddressToJsonString()
        {
            //申請API Key，能夠呼叫的額度會多一些
            string address = "高雄市岡山區大莊路80巷";
            string GoogleAPIKey = System.Configuration.ConfigurationManager.AppSettings["GoogleAPIKey"];
            string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(address, Encoding.UTF8) + "&key=" + GoogleAPIKey;
            string result = "";//回傳結果 
            using (WebClient client = new WebClient())
            {
                //指定語言，否則Google預設回傳英文   
                client.Headers[HttpRequestHeader.AcceptLanguage] = "zh-TW";
                //不設定的話，會回傳中文亂碼
                client.Encoding = Encoding.UTF8;
                #region POST method才會用到
                /*
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] response = client.UploadValues("https://maps.googleapis.com/maps/api/geocode/json", new NameValueCollection()
                {
                        { "address", HttpUtility.UrlEncode(address, Encoding.UTF8)},
                        { "key", GoogleAPIKey }
                });
                result = Encoding.UTF8.GetString(response);
                */
                #endregion
                result = client.DownloadString(url);
            }//end using

            return result;
        }//end method

        // PUT: api/Businesses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusiness(int id, Business business)
        {
            if (id != business.Id)
            {
                return BadRequest();
            }

            _context.Entry(business).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Businesses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<string> PostBusiness(BusinessDTO businessDTO)
        {
            
            Business business = new Business()
            {
                Name = businessDTO.Name,
                Password = businessDTO.Password,
                FkStatusId = businessDTO.status
            };

            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();

            return "新增成功";
        }

        // DELETE: api/Businesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BusinessExists(int id)
        {
            return _context.Businesses.Any(e => e.Id == id);
        }

        //[HttpPost]
        //public async Task <string> GetPosition()
        //{
        //    var address = "820高雄市岡山區大莊路80巷";
        //    var url = String.Format("http://maps.google.com/maps/api/geocode/json?sensor=false&address={0}", address);

        //    string result = String.Empty;
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    using (var response = request.GetResponse())
        //    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
        //    {
        //        //Json格式: 請參考http://code.google.com/intl/zh-TW/apis/maps/documentation/geocoding/
        //        result = sr.ReadToEnd();
        //    }
        //    return result;
        //}




    }
}
