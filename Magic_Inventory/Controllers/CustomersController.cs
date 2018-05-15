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
    
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CustomersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {           
            return View(await _context.Store.ToListAsync());
        }
    }
}