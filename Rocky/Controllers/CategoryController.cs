using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Data;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Category;
            return View(objList);
        }
        //GET Category
        public IActionResult Create()
        {
            return View();
        }
        //POST Category
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Category obj)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //GET Category
        public IActionResult Edit(int? Id)
        {
            if(Id == 0 || Id == null)
            {
                return NotFound();
            }
            var obj = _db.Category.Find(Id);
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST Category
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? CategoryId)
        {
            var obj = _db.Category.Find(CategoryId);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Category.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");


        }

    }
}
