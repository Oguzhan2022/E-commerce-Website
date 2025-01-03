using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Larana.Data;
using Larana.Models;

namespace Larana.Controllers
{
public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;

    public ShopController()
    {
        _context = new ApplicationDbContext();
    }

    public ActionResult Index(string[] category, string[] brand, decimal? minPrice, decimal? maxPrice, string sort = "newest")
    {
        var products = _context.Products.AsQueryable();

        if (category != null && category.Any())
            products = products.Where(p => category.Contains(p.Category));

        if (brand != null && brand.Any())
            products = products.Where(p => brand.Contains(p.Brand));

        if (minPrice.HasValue)
            products = products.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            products = products.Where(p => p.Price <= maxPrice.Value);

        switch (sort)
        {
            case "low-to-high":
                products = products.OrderBy(p => p.Price);
                break;
            case "high-to-low":
                products = products.OrderByDescending(p => p.Price);
                break;
            case "best-sellers":
                products = products.OrderByDescending(p => p.Sales);
                break;
            case "new-products":
                products = products.OrderByDescending(p => p.Id);
                break;
            default:
                products = products.OrderByDescending(p => p.Id);
                break;
        }

        var initialProducts = products.Take(9).ToList();

        ViewBag.Categories = _context.Products.Select(p => p.Category).Distinct().ToList();
        ViewBag.Brands = _context.Products.Select(p => p.Brand).Distinct().ToList();
        ViewBag.SelectedCategories = category ?? new string[] { };
        ViewBag.SelectedBrands = brand ?? new string[] { };
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.Sort = sort;

        return View(initialProducts);
    }

    public ActionResult LoadMoreProducts(int page = 1, int pageSize = 12, string[] category = null, string[] brand = null, decimal? minPrice = null, decimal? maxPrice = null, string sort = "newest")
    {
        var products = _context.Products.AsQueryable();

        if (category != null && category.Any())
            products = products.Where(p => category.Contains(p.Category));

        if (brand != null && brand.Any())
            products = products.Where(p => brand.Contains(p.Brand));

        if (minPrice.HasValue)
            products = products.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            products = products.Where(p => p.Price <= maxPrice.Value);

        switch (sort)
        {
            case "low-to-high":
                products = products.OrderBy(p => p.Price);
                break;
            case "high-to-low":
                products = products.OrderByDescending(p => p.Price);
                break;
            case "best-sellers":
                products = products.OrderByDescending(p => p.Sales);
                break;
            case "new-products":
                products = products.OrderByDescending(p => p.Id);
                break;
            default:
                products = products.OrderByDescending(p => p.Id);
                break;
        }

        products = products.Skip((page - 1) * pageSize).Take(pageSize);

        return PartialView("_ProductList", products.ToList());
    }


        public ActionResult Details(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var relatedProducts = _context.Products
                .Where(p => p.Category == product.Category && p.Id != product.Id)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            string userId = User.Identity.Name;

            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var cartItem = _context.CartItems
                .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Product added to cart successfully." });
        }
        public ActionResult SearchProducts(string query)
        {
            var products = string.IsNullOrWhiteSpace(query)
                ? _context.Products.ToList()
                : _context.Products
                    .Where(p => p.Name.Contains(query) || p.Category.Contains(query) || p.Brand.Contains(query))
                    .ToList();

            return Json(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                p.PhotoPath
            }), JsonRequestBehavior.AllowGet);
        }

}

}