using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = productRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _productRepo.GetAll(includeProperties: "Category,ApplicationType");
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
                CategorySelectList = _productRepo.GetAllDropDownList(WC.CategoryName),
                ApplicationTypeSelectList = _productRepo.GetAllDropDownList(WC.ApplicationTypeName)
            };

            if (Id == null || Id == 0)
            {
                //for Create
                return View(productVM);
            }
            //for update
            else
            {
                productVM.Product = _productRepo.Find(Id.GetValueOrDefault());
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

                    _productRepo.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var objFromDb = _productRepo.FirstOrDefault(u => u.ProductId == productVM.Product.ProductId, isTracking: false);

                    if (files.Count > 0)
                    {

                        string upload = webRootPath + WC.ImagePath;
                        string filename = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var filestream = new FileStream(Path.Combine(upload, filename + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        productVM.Product.Image = filename + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _productRepo.Update(productVM.Product);
                }
                _productRepo.Save();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _productRepo.GetAllDropDownList(WC.CategoryName);
            productVM.ApplicationTypeSelectList = _productRepo.GetAllDropDownList(WC.ApplicationTypeName);
            return View(productVM);
        }

        //GET - DELETE
        public IActionResult Delete(int? Id)
        {

            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Product product = _productRepo.FirstOrDefault(u => u.ProductId == Id, includeProperties: "Category,ApplicationType");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? ProductId)
        {
            var obj = _productRepo.Find(ProductId.GetValueOrDefault());
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
            _productRepo.Remove(obj);
            _productRepo.Save();
            return RedirectToAction("Index");


        }
    }


}


