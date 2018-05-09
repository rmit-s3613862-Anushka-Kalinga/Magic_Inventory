using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magic_Inventory.Models
{
    public class CustomerOrder
    {
        public int CustomerOrderID { get; set; }

        public int OrderDetailID { get; set; }
        public OrderDetail OrdeDetail { get; set; }

        public int StoreID { get; set; }
        public Store Store { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
