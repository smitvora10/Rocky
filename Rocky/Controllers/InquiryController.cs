using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocky_Models.ViewModels;
using Rocky_Models;
using Rocky_Utility;

namespace Rocky.Controllers
{
    public class InquiryController : Controller
    {
        [BindProperty]
        public InquiryVM inquiryVM { get; set; }

        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;

        public InquiryController(IInquiryDetailRepository inqDRepo,
           IInquiryHeaderRepository inqHRepo)
        {
            _inqDRepo = inqDRepo;
            _inqHRepo = inqHRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            inquiryVM = new InquiryVM()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(u => u.InquiryHeaderId == id),
                InquiryDetail = _inqDRepo.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")

            };
            return View(inquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            inquiryVM.InquiryDetail = _inqDRepo.GetAll(u => u.InquiryHeaderId == inquiryVM.InquiryHeader.InquiryHeaderId);

            foreach (var detail in inquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ShoppingProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart,shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, inquiryVM.InquiryHeader.InquiryHeaderId);
            return RedirectToAction("Index","Cart");
        }

        [HttpPost]
        
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.InquiryHeaderId == inquiryVM.InquiryHeader.InquiryHeaderId);
            IEnumerable<InquiryDetail> inquiryDetail = _inqDRepo.GetAll(u => u.InquiryHeaderId == inquiryVM.InquiryHeader.InquiryHeaderId);
            _inqDRepo.RemoveRange(inquiryDetail);
            _inqHRepo.Remove(inquiryHeader);
            _inqHRepo.Save();
            return RedirectToAction(nameof(Index));
        }
        #region API Calls
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new {data = _inqHRepo.GetAll() });
        }
        #endregion
    }
}
