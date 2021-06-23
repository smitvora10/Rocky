using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType);
            //(Another Way)
            //foreach (var obj in objList)
            //{
            //    obj.Category = _db.Category.FirstOrDefault(u => u.CategoryId == obj.CategoryId);
            //}
            return View(objList);
        }
        //GET Product
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ApplicationTypeId.ToString()
                })
            };

            if (Id == null || Id == 0)
            {
                //for Create
                return View(productVM);
            }
            //for update
            else
            {
                productVM.Product = _db.Product.Find(Id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }

        }
        //POST Product
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.ProductId == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _db.Product.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.ProductId == productVM.Product.ProductId);

                    if(files.Count>0)
                    {                       

                        string upload = webRootPath + WC.ImagePath;
                        string filename = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if(System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var filestream = new FileStream(Path.Combine(upload,filename+extension),FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        productVM.Product.Image = filename + extension;
                }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            //productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.CategoryId.ToString()
            //});
            //productVM.ApplicationTypeSelectList = _db.ApplicationType.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.ApplicationTypeId.ToString()
            //});

            return View(productVM);
        }

        //GET - DELETE
        public IActionResult Delete(int? Id)
        {

            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Product product = _db.Product.Include(u => u.Category).Include(u => u.ApplicationType).FirstOrDefault(u => u.ProductId == Id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST - DELETE
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? ProductId)
        {
            var obj = _db.Product.Find(ProductId);
            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;       
            

            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            if (obj == null)
            {
                return NotFound();
            }
            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");


        }
    }


}


