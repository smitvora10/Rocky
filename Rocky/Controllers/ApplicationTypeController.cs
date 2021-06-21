using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objList = _db.ApplicationType;
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
            _db.ApplicationType.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        //GET ApplicationType
        public IActionResult Edit(int ? Id)
        {
            if(Id == 0 || Id == null)
            {
                return NotFound();
            }
            var obj = _db.ApplicationType.Find(Id);
            if(obj == null)
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
            if(ModelState.IsValid)
            {
                _db.ApplicationType.Update(obj);
                _db.SaveChanges();
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
            var obj = _db.ApplicationType.Find(Id);
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
            var obj = _db.ApplicationType.Find(ApplicationTypeId);
            if (ModelState.IsValid)
            {
                _db.ApplicationType.Remove(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
