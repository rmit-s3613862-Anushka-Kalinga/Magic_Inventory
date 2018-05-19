using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Data;
using Magic_Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Magic_Inventory.Controllers
{
   
    public class StoreInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StoreInventoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager){
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = RoleConstants.FranchiseHolderRole)]
        public async Task<IActionResult> Index()
        {
            var user =await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound();
            }
            
            var storeInventoryList = _context.StoreInventory.Include(s => s.Store).Include(o => o.Product).Where(s => s.StoreID == user.StoreID);
            return View( await storeInventoryList.ToListAsync());
        }

        public IActionResult ShowStoreInventory(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var storeProducts =   _context.StoreInventory.Include(o => o.Product).Where(s => s.StoreID == id);

            return View(storeProducts);
        }
    }
}