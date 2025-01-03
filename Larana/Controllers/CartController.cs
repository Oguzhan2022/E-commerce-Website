using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Larana.Data;
using Larana.Models;
using System.Data.Entity;


namespace Larana.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OrdersDbContext _ordersContext;

        public CartController()
        {
            _context = new ApplicationDbContext();
            _ordersContext = new OrdersDbContext();
        }

        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.Name;

            using (var dbAccount = new DbAccount())
            {
                var user = dbAccount.Users.FirstOrDefault(u => u.Username == userId);
                if (user != null)
                {
                    ViewBag.Address = user.Address;
                    ViewBag.PhoneNumber = user.PhoneNumber;
                }
                else
                {
                    ViewBag.Address = "";
                    ViewBag.PhoneNumber = "";
                }
            }

            var cart = _context.Carts
                .Include("CartItems.Product")
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new System.Collections.Generic.List<CartItem>()
                };
            }

            return View(cart);
        }



        [HttpPost]
        [Authorize]
        public ActionResult RemoveFromCart(int productId)
        {
            string userId = User.Identity.Name;

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Product removed from cart." });
                }
            }

            return Json(new { success = false, message = "Failed to remove product from cart." });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(string RecipientName, string ShippingCompany, string Address, string PhoneNumber, string CardInfo)
        {
            string userId = User.Identity.Name;

            var cart = _context.Carts
                .Include(c => c.CartItems.Select(ci => ci.Product))
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Your cart is empty." });
            }

            using (var transaction = _ordersContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var cartItem in cart.CartItems)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                        if (product == null || product.Stock < cartItem.Quantity)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = $"Insufficient stock for product: {cartItem.Product.Name}" });
                        }

                        product.Stock -= cartItem.Quantity;
                        product.Sales += cartItem.Quantity;
                    }

                    _context.SaveChanges();

                    var order = new Order
                    {
                        UserId = userId,
                        CartId = cart.Id,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                        Status = "Pending",
                        Address = Address,
                        ShippingCompany = ShippingCompany,
                        PhoneNumber = PhoneNumber,
                        RecipientName = RecipientName,
                        OrderDetails = cart.CartItems.Select(ci => new OrderDetail
                        {
                            CartItemId = ci.Id,
                            ProductId = ci.ProductId,
                            Quantity = ci.Quantity,
                            UnitPrice = ci.Product.Price
                        }).ToList()
                    };

                    _ordersContext.Orders.Add(order);
                    _ordersContext.SaveChanges();

                    _context.CartItems.RemoveRange(cart.CartItems);
                    _context.Carts.Remove(cart);
                    _context.SaveChanges();

                    transaction.Commit();

                    return Json(new { success = true, message = "Order placed successfully." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
                }
            }
        }

    }
}
