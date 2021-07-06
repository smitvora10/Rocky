using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appTypeRepo;
        public ApplicationTypeController(IApplicationTypeRepository appTypeRepo)
        {
            _appTypeRepo = appTypeRepo;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _appTypeRepo.GetAll();
            return View(objList);
        }
        //GET ApplicationType
        public IActionResult Create()
        {
            return View();
        }
        //POST ApplicationType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Add(obj);
                _appTypeRepo.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //GET ApplicationType
        public IActionResult Edit(int? Id)
        {
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }
            var obj = _appTypeRepo.Find(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST ApplicationType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _appTypeRepo.Update(obj);
                _appTypeRepo.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET ApplicationType
        public IActionResult Delete(int? Id)
        {
            if (Id == 0 || Id == null)
            {
                return NotFound();
            }
            var obj = _appTypeRepo.Find(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST ApplicationType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? ApplicationTypeId)
        {
            var obj = _appTypeRepo.Find(ApplicationTypeId.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            _appTypeRepo.Remove(obj);
            _appTypeRepo.Save();
            return RedirectToAction("Index");

        }
    }
}
