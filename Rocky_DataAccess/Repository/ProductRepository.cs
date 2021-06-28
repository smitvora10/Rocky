using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropDownList(string obj)
        {
            if (obj == WC.CategoryName)
            {
                return _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                });
            }
            if (obj == WC.ApplicationTypeName)
            {
                return _db.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ApplicationTypeId.ToString()
                });
            }
            return null;
        }

        public void Update(Product obj)
        {
            //another method for update
            //_db.Product.Update(obj)
            var objFromDb = base.FirstOrDefault(u => u.ProductId == obj.ProductId);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.ShortDesc = obj.ShortDesc;
                objFromDb.Description = obj.Description;
                objFromDb.Price = obj.Price;
                objFromDb.Image = obj.Image;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.ApplicationTypeId = obj.ApplicationTypeId;
            }
        }
    }
}
