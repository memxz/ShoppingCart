using Microsoft.AspNetCore.Mvc;
using ASPSession.Security;
using ASPSession.Models.ViewModels;
using Newtonsoft.Json;

namespace ASPSession.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] 
    public class UserAccessController : Controller
    {
        ISecurity sec;
        public UserAccessController(ISecurity sec)
        {
            this.sec = sec;
        }

        
        public IActionResult LoginPage()
        {
            if (sec.Authenticate()) return RedirectToAction("Main","Gallery");
            return View();
        }

        [HttpPost]
        public JsonResult Verify(string Username, string Password)  //takes in data from ajax script to verify user
        {
            return Json(Login(Username, Password)); // returns boolean value in a json object to the view

        }

        private bool Login(string Username, string Password)
        {

            string? customerID;
            if (sec.LoginAuthentication(Username, Password, out customerID)) //authenticate credentials, will provide customerID if true
            {
                sec.ProvideSession(Username, customerID!); //if authenticated, provide with a session to allow access to authorized pages

                string? nullStr;
                if (Request.Cookies.TryGetValue(customerID!, out nullStr) && Request.Cookies.TryGetValue("404", out nullStr)) //check if both carts when logged in and when not logged in has items (TryGetValue method wont throw an exception if key is not found)
                {
                    List<CartItem> combinedList = CombineCartItems(customerID, "404"); //if have, to combine the items
                    Response.Cookies.Append(customerID!, JsonConvert.SerializeObject(combinedList)); //store new list in cookie
                }
                else if (Request.Cookies.TryGetValue("404"!, out nullStr)) //if the unlogged-in cart only has items, then allow the logged in cart to take over the items
                {
                    Response.Cookies.Append(customerID!, Request.Cookies["404"]!);
                }
                Response.Cookies.Delete("404"); //once user logs-in and taken over the product values from the temp storage, delete the cookie so the temporary cart view will be empty for other non logged in users to use

                return true;
            }
            else return false;

        }


        public IActionResult Invalid(int id)
        {
            if (sec.Authenticate()) return RedirectToAction("Main", "Gallery");
            return View();
        }


        public IActionResult Logout()
        {
            sec.RemoveAuthentication();
            return RedirectToAction("LoginPage","UserAccess");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
        private List<CartItem> CombineCartItems(string? customerID, string? tempID)  // to prevent duplicates when customer prior to login places some items into shopping cart and at the same time before logging in he has already some items in his shopping cart, when combining the list it is possible to have duplicate items
                                                    // so this method is to combine the inner quantities of each CartItem and remove the duplicate
        {
            
            List<CartItem> combinedList = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[tempID]);   //get cart items stored from a temp cookie
            List<CartItem> list2 = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID]); //get cart items stored from logged in user

        
         
            combinedList.AddRange(list2); //combine both list (doesnt matter if 1 or both lists are empty)

            combinedList = combinedList.OrderBy(x => x.ProductId).ToList();    //sort the items according to product ID

            for (int i = 0; i < combinedList.Count()-1; i++) // for each similar product, to combine their quantitites
            {
                if (combinedList[i].ProductId == combinedList[i+1].ProductId)
                {
                    combinedList[i + 1].Quantity += combinedList[i].Quantity;
                    combinedList[i].Quantity = 0;    //turn the duplicate's quantity to zero so that it will be removed later on
                }
            }

            combinedList.RemoveAll(x => x.Quantity == 0); //removes all zero quantity items so there is no duplicates
            return combinedList;

        }


    }

  
}
