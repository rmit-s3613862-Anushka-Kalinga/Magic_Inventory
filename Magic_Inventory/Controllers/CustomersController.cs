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

            //For searching by store
            var StoreLst = new List<string>();
            var StoreQry = _context.Store.Select(m => m.Name);
            StoreLst.AddRange(StoreQry.Distinct());            
            ViewBag.StoreName = new SelectList(StoreLst,store);

            var applicationDbContext = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(d => d.StockLevel > 0);
            if (!String.IsNullOrEmpty(searchName))
            {
                ViewData["searchName"] = searchName;
                if (!String.IsNullOrEmpty(store))
                {
                    ViewData["store"] = store;
                    var applicationDbContext3 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Product.Name.Contains(searchName)).Where(h=>h.Store.Name == store).Where(d=>d.StockLevel >0);
                    return View(await applicationDbContext3.ToListAsync());
                }
                
                var applicationDbContext2 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Product.Name.Contains(searchName)).Where(d => d.StockLevel > 0);
                return View(await applicationDbContext2.ToListAsync());
            }
            if (!String.IsNullOrEmpty(store))
            {
                ViewData["store"] = store;
                var applicationDbContext2 = _context.StoreInventory.Include(s => s.Product).Include(m => m.Store).Where(s => s.Store.Name == store).Where(d => d.StockLevel > 0);
                return View(await applicationDbContext2.ToListAsync());
            }

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> AddToCart(int? Quantity,int? ProductID, int? StoreID,string searchName,string store)
        {
            Console.WriteLine(searchName);
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
            return RedirectToAction("Index",new { @searchName = searchName,@store = store});
        }
    }
}
