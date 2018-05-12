using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Data;
using Magic_Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Magic_Inventory.Controllers
{
    [Authorize(Roles = RoleConstants.OwnerRole)]
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


        // GET: OwnerInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
                // return NotFound();
            }          
            var ownerInventory = await _context.OwnerInventory.Include(o => o.Product).SingleOrDefaultAsync(m => m.ProductID == id);
            if (ownerInventory == null)
            {
                return NotFound();
            }
            // ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        // POST: OwnerInventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,StockLevel")] OwnerInventory ownerInventory)
        {
            if (id != ownerInventory.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownerInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerInventoryExists(ownerInventory.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        private bool OwnerInventoryExists(int id)
        {
            return _context.OwnerInventory.Any(e => e.ProductID == id);
        }

    }
}