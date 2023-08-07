using ASPSession.DAO.Interfaces;
using ASPSession.Security;
using Microsoft.AspNetCore.Mvc;

namespace ASPSession.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class RatingController : Controller
    {

        private readonly ISecurity sec;
        private readonly IHttpContextAccessor context;
        private readonly IRatingDB rtdb;


        public RatingController(ISecurity sec, IHttpContextAccessor context, IRatingDB rtdb)
        {
            this.sec = sec;
            this.context = context;
            this.rtdb = rtdb;
        }

        public IActionResult Index(int productId)
        {
            if (!sec.Authenticate()) return RedirectToAction("AccessDenied","UserAccess");
            ViewData["productId"] = productId;
            ViewData["Username"] = context.HttpContext!.Session.GetString("Username");
            return View();
        }


        public string SetStarRating(int UserId, int ProductId, int Rating)
        {
            bool status = SetStar(UserId, ProductId, Rating);
            return status ? "success" : "fail";
        }

        public bool SetStar(int cid, int pid, int rid)
        {
            if (rtdb.UpdateStar(cid, pid, rid))
            {
                return true;
            }

            return rtdb.AddStar(cid, pid, rid);
        }

    }
}


