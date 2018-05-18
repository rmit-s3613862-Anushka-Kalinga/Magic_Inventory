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
        private string OrderNumber = "051913170518c1@sys.com";

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index(int? cartID,String val)
        {
            ViewData["CardV"] = CreditCardVerified;
            Cart newQuantity = new Cart();
            if (! String.IsNullOrEmpty(val))
                if (cartID != null)
                {
                    try
                    {
                        newQuantity = _context.Cart.Where(s => s.CartID == cartID).Single();
                    }
                    catch
                    {
                        var applicationDbContext2 = _context.Cart.Include(c => c.Product).Include(c => c.Store);
                        return View(await applicationDbContext2.ToListAsync());
                    }
                    if (val.Equals("+"))
                        newQuantity.Quantity++;
                    else if (val.Equals("-"))
                        newQuantity.Quantity--;
                    if (newQuantity.Quantity <= 0)
                        _context.Cart.Remove(newQuantity);
                    else
                        _context.Update(newQuantity);
                    _context.SaveChanges();
                    val = "";
                }
            var applicationDbContext = _context.Cart.Include(c => c.Product).Include(c => c.Store).Where(l=>l.UserName == User.Identity.Name.ToString());
            int totalitem = 0;
            double totalcost =0;
            foreach (var item in applicationDbContext)
            {
                totalitem += item.Quantity;
                totalcost = totalcost + (item.Quantity * item.Price);
            }
            ViewData["item"] = totalitem;
            ViewData["cost"] = totalcost;
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
                return Redirect(nameof(Invoice));
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
                OrderNumber = orderNo;
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

        //[ValidateAntiForgeryToken]
        public IActionResult Invoice()
        {
            if (!OrderNumber.Equals(""))
            {
                var context2 = _context.OrderHistory.Include(z => z.Product).Include(n => n.Store).Where(m => m.OrderNumber == "051913170518c1@sys.com");
                return View(context2);
            }
            var context = _context.OrderHistory.Include(z => z.Product).Include(n => n.Store).Where(m => m.OrderNumber == "051913170518c1@sys.com");
            return View(context);
            return NotFound();
        }

       
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
        public async Task<IActionResult> DeleteItem(int? id)
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
        public async Task<IActionResult> DeleteConfirmed(int CartID)
        {
            var cart = await _context.Cart.SingleOrDefaultAsync(m => m.CartID == CartID);
            if (cart != null)
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
