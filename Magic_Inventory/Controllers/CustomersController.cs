using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Magic_Inventory.Data;
using Magic_Inventory.Models;
using Microsoft.AspNetCore.Authorization;

namespace Magic_Inventory.Controllers
{
    [Authorize(Roles = RoleConstants.CustomerRole)]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string searchName,string store)
        {
            ViewData["searchName"] = "";
            ViewData["store"] = "";

            var applicationDbContext = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store);
            if (!String.IsNullOrEmpty(searchName))
            {
                ViewData["searchName"] = searchName;
                if (!String.IsNullOrEmpty(store))
                {
                    ViewData["store"] = store;
                    var applicationDbContext3 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Product.Name.Contains(searchName)).Where(h=>h.Store.Name == store);
                    return View(await applicationDbContext3.ToListAsync());
                }
                
                var applicationDbContext2 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Product.Name.Contains(searchName));
                return View(await applicationDbContext2.ToListAsync());
            }
            if (!String.IsNullOrEmpty(store))
            {
                ViewData["store"] = store;
                var applicationDbContext2 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Store.Name == store);
                return View(await applicationDbContext2.ToListAsync());
            }

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> AddToCart(int? Quantity,int? ProductID, int? StoreID)
        {
            if (ProductID == null || StoreID == null || Quantity == null)
            {
                return NotFound();
            }
            var temp = _context.Cart.Where(z => z.UserName == User.Identity.Name.ToString()).Where(c => c.ProductID == (int)ProductID).Where(m => m.StoreID == StoreID).SingleOrDefault();
            if (temp == null)
            {
                var cartItem = new Cart();
                cartItem.StoreID = (int)StoreID;
                cartItem.ProductID = (int)ProductID;
                cartItem.Quantity = (int)Quantity;
                cartItem.UserName = User.Identity.Name.ToString();
                cartItem.Price = await _context.Product.Where(c => c.ProductID == (int)ProductID).Select(c => c.Price).SingleOrDefaultAsync();
                cartItem.CartEntryDate = DateTime.Today;
                _context.Cart.Add(cartItem);                
            }
            else
            {
                temp.Quantity += (int)Quantity;
            }
            var reduceTemp = _context.StoreInventory.Where(p => p.ProductID == (int)ProductID).Where(m => m.StoreID == (int)StoreID).SingleOrDefault();
            reduceTemp.StockLevel = reduceTemp.StockLevel - (int)Quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------- To be Remove -----------------------------------------
        // GET: Customers/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var storeInventory = await _context.StoreInventory
        //        .Include(s => s.Product)
        //        .Include(s => s.Store)
        //        .SingleOrDefaultAsync(m => m.StoreID == id);
        //    if (storeInventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(storeInventory);
        //}

        // GET: Customers/Create
        //public IActionResult Create()
        //{
        //    ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name");
        //    ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(storeInventory);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", storeInventory.ProductID);
        //    ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
        //    return View(storeInventory);
        //}

        // GET: Customers/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var storeInventory = await _context.StoreInventory.SingleOrDefaultAsync(m => m.StoreID == id);
        //    if (storeInventory == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", storeInventory.ProductID);
        //    ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
        //    return View(storeInventory);
        //}
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        //{
        //    if (id != storeInventory.StoreID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(storeInventory);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!StoreInventoryExists(storeInventory.StoreID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", storeInventory.ProductID);
        //    ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", storeInventory.StoreID);
        //    return View(storeInventory);
        //}

        // GET: Customers/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var storeInventory = await _context.StoreInventory
        //        .Include(s => s.Product)
        //        .Include(s => s.Store)
        //        .SingleOrDefaultAsync(m => m.StoreID == id);
        //    if (storeInventory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(storeInventory);
        //}

        // POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var storeInventory = await _context.StoreInventory.SingleOrDefaultAsync(m => m.StoreID == id);
        //    _context.StoreInventory.Remove(storeInventory);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool StoreInventoryExists(int id)
        //{
        //    return _context.StoreInventory.Any(e => e.StoreID == id);
        //}
    }
}
