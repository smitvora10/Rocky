using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Models
{
    public class Category
    {
        [Key]
        public int CategoryId {get; set;}
        [Required]        
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="Enter Display Order in Numerical value only")]
        public int DisplayOrder { get; set; }

    }
}
