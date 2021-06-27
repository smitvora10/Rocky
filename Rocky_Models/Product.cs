using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(0,int.MaxValue)]
        public int Price { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category Name")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public int ApplicationTypeId { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }


    }
}
