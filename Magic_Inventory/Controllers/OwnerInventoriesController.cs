using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Magic_Inventory.Controllers
{
    public class OwnerInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerInventoriesController(ApplicationDbContext context) {
            _context = context;

        }


        public async Task<IActionResult> Index()
        {
            var ownerInventoryList = _context.OwnerInventory.Include(o => o.Product);
            return View(await ownerInventoryList.ToListAsync());
        }
    }
}