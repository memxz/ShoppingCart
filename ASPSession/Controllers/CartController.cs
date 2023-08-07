using Microsoft.AspNetCore.Mvc;
using ASPSession.Models;
using ASPSession.Models.ViewModels;
using Newtonsoft.Json;
using ASPSession.DAO.Interfaces;

namespace ASPSession.Controllers;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] //does not cache login page after logged-in, forces browser to request from server a new page, server will check and prevent user from viewing login page if already logged in
public class CartController : Controller
{
    private IOrderHistoryDB odb;
    private readonly ICartDB cdb;
    private readonly IHttpContextAccessor context;
    private readonly string? customerID;

    public CartController(IHttpContextAccessor context, IOrderHistoryDB odb, ICartDB cdb) // IOrderHistoryDB is a custom interface stored in the DI container
    {
        this.cdb = cdb;
        this.odb = odb;
        this.context = context;
        string? cust = context.HttpContext!.Session.GetString("customerID");    // first gets customerID from session, if not logged in return null
        customerID = cust == null || cust == "" ? "404" : context.HttpContext!.Session.GetString("customerID");  //provide a tempID(e.g.,404) for unlogged-in customers
       
    }

    public IActionResult Index()
    {
        List<CartItem> cart;    
        if (Request.Cookies[customerID!] == null) // to check if the return value (json serialized list that is converted to a string) is null
        { 
            cart = new List<CartItem>(); //if return value is null to provide a list and later to convert it to a json serialized string (this list will be the list used to store cart items)
        }
        else
            cart = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID!]); //if the return value is not null, to convert the json string back into a list
        

        CartViewModel cartVM = new()   // now store the list of cart items into the cartviewmodel object, together with the total sum of all the items in the list
        {
            CartItems = cart,
            GrandTotal = cart.Sum(x => x.Quantity * x.Price)
        };
        ViewData["Username"] = context.HttpContext!.Session.GetString("Username"); // storing username in viewdata so that it can be used inside the cart view, e.g., to display a greeting using customer's username
        return View(cartVM);
    }

    public IActionResult Add(int id)
    {
        Product product = odb.GetProductByID(id);   // whenever the user clicks on the add button in cart view, or the price button of an item in gallery view, it returns product object
       
        List<CartItem> cart;
        if (Request.Cookies[customerID!] == null) // if customer, either logged-in or unlogged-in, does not have a temporary "cart list", to provide them with a "cart list"
        {
            cart = new List<CartItem>();
        }
        else
            cart = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID!]); // if they have an existing "cart list", to convert the json string back to a list

        CartItem? cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();  // now this step is crucial to prevent product image duplicates, for instance if a customer buys more than 1 same product, 
                                                                                   // the number of products of that type will flood the cart view screen, to prevent this, each individual product has a 'quantity' property
                                                                                   // hence whenever the same quantity value of a product increase, only the 'quantity' property value increases - the cart view will only show 1 product image with adjustable quantities
                                                                                   
        if (cartItem == null) // if the existing list does not have the product to be added
        {
            cart.Add(new CartItem(product));  //add the product into the list
        }
        else
        {
            cartItem.Quantity += 1;  // if the product going to be added (into the list) is found inside the list, only increase the 'quantity' value of the product inside the list
        }

        Response.Cookies.Append(customerID!, JsonConvert.SerializeObject(cart)); //embed the new list (after serializing it into a json string) into a cookie, the key will be the customer ID (either a real ID or temp ID)

        return Redirect(Request.Headers["Referer"].ToString()); 
    }
    public IActionResult Decrease(int id)  // same thing here with the decrease process - its the opposite of increase
    {
       
        List<CartItem> cart = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID!]);

        CartItem? cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

        if (cartItem.Quantity > 1)
        {
            --cartItem.Quantity;
        }
        else
        {
            cart.RemoveAll(p => p.ProductId == id);
        }

        if (cart.Count == 0)
        {
            
            Response.Cookies.Delete(customerID!);
        }
        else
        {
           
            Response.Cookies.Append(customerID!, JsonConvert.SerializeObject(cart));
        }

        TempData["Success"] = "The product has been removed!";

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Remove(long id) //removes the entire product from the "cart list"
    {
        
        List<CartItem> cart = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID!]); // when remove button pressed in cart view, it sends the product ID collected from cart view to this method, using the helper tag "asp-routeId"
        
        cart.RemoveAll(p => p.ProductId == id); // search for product inside current list and remove it

        Response.Cookies.Append(customerID!, JsonConvert.SerializeObject(cart)); //store the modified cart inside the cookie 

        return RedirectToAction("Index");
    }

    public IActionResult Clear() //clears the whole cart
    {
        RemoveCartCookies();

        return RedirectToAction("Index");
    }
    private void RemoveCartCookies()
    {
        Response.Cookies.Delete(customerID!);
    }


    public IActionResult CheckOut()
    {   
        List<CartItem> cart = JsonConvert.DeserializeObject<List<CartItem>>(Request.Cookies[customerID!]);

        List<Orders> orders = new List<Orders>();
        if (cdb.AddOrderById(cart, Convert.ToInt32(customerID)) > 0) // records the products bought into the database
        {
            if (odb.GetOrderByCustomer(customerID, ref orders) && cdb.AddActivationCode(Convert.ToInt32(customerID)))  //creates activation codes and adds them into the database, for every single item
            {
                RemoveCartCookies();
            }

        }

        return RedirectToAction("MyPurchases", "OrderHistory");
    }


    
}


