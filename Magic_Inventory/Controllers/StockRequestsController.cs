using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Data;
using Magic_Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Magic_Inventory.Controllers
{
    public class StockRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var stockRequestsList = _context.StockRequest.Include(s => s.Store).Include(o => o.Product);
            return View(await stockRequestsList.ToListAsync());
        }

        public IActionResult Create(int? StoreID, int? ProductID) {

            // var x = _context.OwnerInventory.Include(s => s.StockLevel);
            if (StoreID == null && ProductID == null) {
                return NotFound();
            }

            return View("Home");
        }
    }
}