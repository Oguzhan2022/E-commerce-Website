using System;
using System.Linq;
using System.Web.Mvc;
using Larana.Data;
using Larana.Models;
using System.Collections.Generic;

namespace Larana.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }
        [Route("Index")] //it is domain/Home/Index because of Prefix Routing
        [Route("~/")] //~means domain and it is domain's itself
        public ActionResult Index()
        {
            var newProducts = _context.Products
                                      .OrderByDescending(p => p.Id)
                                      .Take(9)
                                      .ToList();

            var bestSellers = _context.Products
                                      .OrderByDescending(p => p.Sales)
                                      .Take(9)
                                      .ToList();

            ViewBag.NewProducts = newProducts;
            ViewBag.BestSellers = bestSellers;

            return View();
        }

        [Route("About")] //it is domain/Home/About because of Prefix Routing
        [Route("~/About")] //~means domain and it is domain/About
        public ActionResult About()
        {
            return View();
        }
        [Route("Contact")] //it is domain/Home/Contact because of Prefix Routing
        [Route("~/Contact")] //~means domain and it is domain/Contact
        public ActionResult Contact()
        {
            return View();
        }
        [Route("FAQ")] //it is domain/Home/FAQ because of Prefix Routing
        [Route("~/FAQ")] //~means domain and it is domain/FAQ
        public ActionResult FAQ()
        {
            return View();
        }
    }
}