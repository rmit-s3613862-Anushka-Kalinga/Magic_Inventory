using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Magic_Inventory.Models
{
    public class OwnerInventory
    {
        [Key]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name ="Stock Level")]
        public int StockLevel { get; set; }
    }
}
