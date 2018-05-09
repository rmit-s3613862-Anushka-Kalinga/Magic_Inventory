using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Magic_Inventory.Models
{
    public class StoreInventory
    {

        public int StoreID { get; set; }
        public Store Store { get; set; }


        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int StockLevel { get; set; }
    }
}
