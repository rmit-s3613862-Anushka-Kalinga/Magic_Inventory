using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Magic_Inventory.Data;
using Magic_Inventory.Models;

namespace Magic_Inventory.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private bool CreditCardVerified = false;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            ViewData["CardV"] = CreditCardVerified;
            var applicationDbContext = _context.Cart.Include(c => c.Product).Include(c => c.Store);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCreditCard()
        {
            //var applicationDbContext = _context.Cart.Include(c => c.Product).Include(c => c.Store);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCreditCardNow()
        {
            if (!CreditCardVerified)
            {
                CreditCardVerified = true;
                //var applicationDbContext = _context.Cart.Include(c => c.Product).Include(c => c.Store);
                // Verify credit card and then make changes to the table
                bool result = await UpdateCartAndOrderHistoryAsync();
                return Redirect(nameof(Index));
            }
            else
                return NotFound();
        }

        private async Task<bool> UpdateCartAndOrderHistoryAsync()
        {
            if (CreditCardVerified)
            {                
                var cartItem = _context.Cart.Where(s => s.UserName == User.Identity.Name.ToString());
                
                var orderNo = new OrderHistory().OrderNumber;
                orderNo = DateTime.Now.ToString("hhmmssddMMyy") + User.Identity.Name.ToString();
                foreach (var item in cartItem)
                {
                    var orderHistory = new OrderHistory();
                    orderHistory.OrderDate = DateTime.Now;
                    orderHistory.ProductID = item.ProductID;
                    orderHistory.StoreID = item.StoreID;
                    orderHistory.Quantity = item.Quantity;
                    orderHistory.UserName = item.UserName;
                    orderHistory.Price = item.Price;
                    orderHistory.OrderNumber = orderNo;
                    _context.OrderHistory.Add(orderHistory);
                    _context.Cart.Remove(item);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Invoice()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderHistory()
        {

            return View();
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Product)
                .Include(c => c.Store)
                .SingleOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name");
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartID,UserName,ProductID,StoreID,Quantity,Price,CartEntryDate")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", cart.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", cart.StoreID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.SingleOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", cart.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", cart.StoreID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartID,UserName,ProductID,StoreID,Quantity,Price,CartEntryDate")] Cart cart)
        {
            if (id != cart.CartID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartID))
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
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "Name", cart.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Store, "StoreID", "StoreID", cart.StoreID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Product)
                .Include(c => c.Store)
                .SingleOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.SingleOrDefaultAsync(m => m.CartID == id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartID == id);
        }
    }
}
