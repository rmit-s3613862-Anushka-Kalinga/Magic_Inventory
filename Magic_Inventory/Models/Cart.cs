using System;
using System.Collections.Generic;
using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Magic_Inventory.Models
{
    public class Cart

    {
        [Key]
        public int CartID { get; set; }

        //Email id in this case.
        [Required]
        public string UserName { get; set; }

        [ForeignKey("Product"),Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Store"),Required]
        public int StoreID { get; set; }
        public Store Store { get; set; }

        [Min(0),Required]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000.00")]
        public double Price { get; set; }

        [DataType(DataType.Date),Required,Display(Name ="Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CartEntryDate { get; set; }        
    }
}
