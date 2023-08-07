using ASPSession.DAO.Interfaces;
using ASPSession.Models;
using ASPSession.Security;
using Microsoft.AspNetCore.Mvc;

namespace ASPSession.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class OrderHistoryController : Controller
    {
        private readonly ISecurity sec;
        private readonly IOrderHistoryDB odb;
        private readonly IHttpContextAccessor context;

        public OrderHistoryController(ISecurity sec, IHttpContextAccessor context, IOrderHistoryDB odb)
        {
            this.odb = odb;
            this.sec = sec;
            this.context = context;

        }
        public IActionResult MyPurchases()
        {
            if (!sec.Authenticate()) return RedirectToAction("AccessDenied","UserAccess");
            List<Orders> orders = new List<Orders>();
            if (odb.GetOrderByCustomer(context.HttpContext!.Session.GetString("customerID"), ref orders)) //if returns false, skips the following processes and return empty view
            {
                Dictionary<int, List<ActivationCode>> activationCodes =
                    new Dictionary<int, List<ActivationCode>>();
                List<Product> ProductLst = new List<Product>();
                for (int i = 0; i < orders.Count; i++)
                {
                    int orderID = orders[i].OrderId;
                    activationCodes[orderID] = odb.GetActivationCodesByOrder(orderID);
                    int productID = orders[i].ProductId;
                    Product product = odb.GetProductByID(productID);
                    ProductLst.Add(product);
                }
                ViewData["Orders"] = orders;
                ViewData["ActivationCode"] = activationCodes;
                ViewData["Product"] = ProductLst;
                ViewData["Username"] = context.HttpContext!.Session.GetString("Username");
            }

            return View();
        }

    }
}