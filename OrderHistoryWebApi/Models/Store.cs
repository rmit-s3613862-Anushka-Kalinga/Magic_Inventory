using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryWebApi.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }

        public string Name { get; set; }
    }
}
