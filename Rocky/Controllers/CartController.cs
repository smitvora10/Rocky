using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ProductUserVM productUserVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;     
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ShoppingProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.ProductId));

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ShoppingProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.ProductId));

            productUserVM = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId),
                ProductList = prodList.ToList()
            };
            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM productUserVM)
        {
            
            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
        public IActionResult Remove(int? Id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ShoppingProductId == Id));

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }        
    }
}
