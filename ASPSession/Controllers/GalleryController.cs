using ASPSession.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using ASPSession.Models.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ASPSession.DAO.Interfaces;

namespace ASPSession.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class GalleryController : Controller
    {
        private readonly IGalleryDB gdb;
        private readonly IRatingDB rtdb;
        private readonly IHttpContextAccessor context;


        public GalleryController(IGalleryDB gdb, IRatingDB rtdb, IHttpContextAccessor context)
        {
            this.rtdb = rtdb;
            this.gdb = gdb;
            this.context = context;

        }


        public IActionResult Main() 
        {
            float totalProducts = gdb.GetTotalProducts();
            Dictionary<int, Rating> allRatingsList = new Dictionary<int, Rating>();
            for (int i = 1; i <= totalProducts; i++)
            {
                Rating rating = rtdb.GetAvgRating(i);
                allRatingsList.Add(i, rating);
            }
            ViewData["allRatingsList"] = allRatingsList;

            List<Product> products = gdb.GetAllProducts();
            ViewData["products"] = products;
            ViewData["Username"] = context.HttpContext!.Session.GetString("Username");

            return View();
        }


        public IActionResult Search(string searchStr)
        {
            float totalProducts = gdb.GetTotalProducts();
            Dictionary<int, Rating> allRatingsList = new Dictionary<int, Rating>();
            for (int i = 1; i <= totalProducts; i++)
            {
                Rating rating = rtdb.GetAvgRating(i);
                allRatingsList.Add(i, rating);
            }
            ViewData["allRatingsList"] = allRatingsList;

            List<Product> products = gdb.GetSearchProducts(searchStr);
            ViewData["products"] = products;
            ViewData["searchStr"] = searchStr;
            ViewData["Username"] = context.HttpContext!.Session.GetString("Username");
            return View("Main");
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public JsonResult GetCartCount()
        {
            List<CartItem> list = new List<CartItem>();
            string? cookieStr = Request.Cookies[context.HttpContext!.Session.GetString("customerID") ??  "404"];
            if (cookieStr != null)
            {
                list = JsonConvert.DeserializeObject<List<CartItem>>(cookieStr);
            }
                int length = list.Sum(x => x.Quantity);
            
            return Json(length);
        }


        [Authorize] //find out what is kept private! Try removing tag and key in url >:)
        public IActionResult Privacy()
        {
            return View();
        }
    }



}