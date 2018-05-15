using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Magic_Inventory.Controllers
{
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;
      


        public StoresController(ApplicationDbContext context)
        {
            _context = context;
           
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Store.ToListAsync());
        }
    }
}