using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IProductRepository _prodRepo;
        private readonly IApplicationUserRepository _appUserRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        [BindProperty]
        public ProductUserVM productUserVM { get; set; }

        public CartController(IProductRepository productRepo, IApplicationUserRepository appUserRepo, IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo
            , IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _prodRepo = productRepo;
            _appUserRepo = appUserRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
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
            IEnumerable<Product> prodListTemp = _prodRepo.GetAll(u => prodInCart.Contains(u.ProductId));
            IList<Product> prodList = new List<Product>();

            foreach (var cartObj in shoppingCartList)
            {
                Product productTemp = prodListTemp.FirstOrDefault(u => u.ProductId == cartObj.ShoppingProductId);
                productTemp.TempSqFt = cartObj.SqFt;
                prodList.Add(productTemp);
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (var obj in prodList)
            {
                shoppingCartList.Add(new ShoppingCart { ShoppingProductId = obj.ProductId, SqFt = obj.TempSqFt });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            ApplicationUser applicationUser;
            
            if(User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.InquiryHeaderId == HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        FullName = inquiryHeader.FullName,
                        Email = inquiryHeader.Email,
                        PhoneNumber = inquiryHeader.PhoneNumber                       

                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                applicationUser = _appUserRepo.FirstOrDefault(u => u.Id == userId);
            }


            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ShoppingProductId).ToList();
            IEnumerable<Product> prodList = _prodRepo.GetAll(u => prodInCart.Contains(u.ProductId));

            productUserVM = new ProductUserVM()
            {
                ApplicationUser = applicationUser,                
            };

            foreach(var obj in shoppingCartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.ProductId == obj.ShoppingProductId);
                prodTemp.TempSqFt = obj.SqFt;
                productUserVM.ProductList.Add(prodTemp);
            }

            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(ProductUserVM productUserVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            //  + "templates" + Path.DirectorySeparatorChar.ToString() +
            //  "Inquiry.html";

            //var subject = "New Inquiry";
            //string HtmlBody = "";
            //using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            //{
            //    HtmlBody = sr.ReadToEnd();
            //}
            ////Name: { 0}
            ////Email: { 1}
            ////Phone: { 2}
            ////Products: {3}


            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = userId,
                InquiryDate = DateTime.Now.Date,
                PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                FullName = productUserVM.ApplicationUser.FullName,
                Email = productUserVM.ApplicationUser.Email
            };
            _inqHRepo.Add(inquiryHeader);
            _inqHRepo.Save();            


            foreach (var prod in productUserVM.ProductList)
            {

                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.InquiryHeaderId,
                    ProductId = prod.ProductId

                };
                _inqDRepo.Add(inquiryDetail);

            }
            _inqDRepo.Save();
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

        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach(var obj in prodList)
            {
                shoppingCartList.Add(new ShoppingCart { ShoppingProductId = obj.ProductId, SqFt = obj.TempSqFt });
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
