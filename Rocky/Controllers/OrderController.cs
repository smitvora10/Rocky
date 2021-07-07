using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo, IBrainTreeGate brain)
        {
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }
        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM()
            {
                OrderHList = _orderHRepo.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i=> new SelectListItem
                {
                    Text=i,
                    Value=i
                })
            };
            if(!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u=>u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status!="--Order Status--")
            {
                orderListVM.OrderHList = orderListVM.OrderHList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }
            return View(orderListVM);
        }

        public IActionResult Details(int ?Id)
        {
            orderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(u => u.OrderHeaderId == Id),
                OrderDetail = _orderDRepo.GetAll(u => u.OrderHeaderId == Id,includeProperties:"Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderHRepo.Save();
            TempData[WC.Success] = "Order is In Process";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepo.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);

            var gateway = _brain.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);

            if(transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                Result<Transaction> resultvoid = gateway.Transaction.Refund(orderHeader.TransactionId);
            }
            orderHeader.OrderStatus = WC.StatusRefunded;
            _orderHRepo.Save();
            TempData[WC.Success] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHRepo.FirstOrDefault(u => u.OrderHeaderId == orderVM.OrderHeader.OrderHeaderId);
            orderHeaderFromDb.FullName = orderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.State = orderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = orderVM.OrderHeader.Email;

            _orderHRepo.Save();
            TempData[WC.Success] = "Order Details Updated Successfully";

            return RedirectToAction("Details", "Order", new { id = orderHeaderFromDb.OrderHeaderId });
        }
    }
}
