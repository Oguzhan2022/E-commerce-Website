using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Larana.Models;
using Larana.Data;
using System.Collections.Generic;


namespace Larana.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private DbAccount db = new DbAccount();


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyAccount", "Account");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    user.Username,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    false,
                    user.Roles,
                    FormsAuthentication.FormsCookiePath
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                if (user.Roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("MyAccount", "Account");
            }
            else
            {
                ViewBag.Message = "Invalid Username or Password";
                return View();
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyAccount", "Account");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;

                user.Roles = string.IsNullOrEmpty(user.Roles) ? "User" : user.Roles;

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }


        [HttpGet]
        public ActionResult Edit()
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User updatedUser, string currentPassword, string newPassword)
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View(updatedUser);
            }

            if (!string.IsNullOrWhiteSpace(currentPassword) && user.Password != currentPassword)
            {
                ViewBag.Message = "Current password is incorrect.";
                return View(updatedUser);
            }

            user.Email = updatedUser.Email;
            user.Address = updatedUser.Address;
            user.PhoneNumber = updatedUser.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updatedUser.Username) && updatedUser.Username != user.Username)
            {
                user.Username = updatedUser.Username;
                FormsAuthentication.SetAuthCookie(user.Username, false);
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Password = newPassword;
            }

            db.SaveChanges();
            ViewBag.Message = "Your account information has been successfully updated.";
            return View(updatedUser);
        }

        public ActionResult MyAccount()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        public ActionResult ViewOrders()
        {
            var username = User.Identity.Name;

            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return HttpNotFound("User not found.");
            }

            using (var ordersContext = new OrdersDbContext())
            {
                var orders = ordersContext.Orders
                    .Where(o => o.UserId == user.Username)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        RecipientName = o.RecipientName,
                        ShippingCompany = o.ShippingCompany,
                        Address = o.Address,
                        PhoneNumber = o.PhoneNumber,
                        OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                        {
                            ProductId = od.ProductId,
                            ProductName = null,
                            Quantity = od.Quantity,
                            UnitPrice = od.UnitPrice
                        }).ToList()
                    }).ToList();

                using (var appContext = new ApplicationDbContext())
                {
                    var productIds = orders.SelectMany(o => o.OrderDetails).Select(od => od.ProductId).Distinct().ToList();
                    var productDetails = appContext.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToDictionary(p => p.Id, p => p.Name);

                    foreach (var order in orders)
                    {
                        foreach (var detail in order.OrderDetails)
                        {
                            detail.ProductName = productDetails.ContainsKey(detail.ProductId) ? productDetails[detail.ProductId] : "Unknown Product";
                        }
                    }

                    return View(orders);
                }
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}

public class OrderDetailViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
public class OrderViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public string RecipientName { get; set; }
    public string ShippingCompany { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public List<OrderDetailViewModel> OrderDetails { get; set; }
}


