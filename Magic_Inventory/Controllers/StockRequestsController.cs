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
            var stockRequestsList =_context.StockRequest.Include(s => s.Store).Include(o => o.Product);
            return View(await stockRequestsList.ToListAsync());
        }


        //Return stock requests corresponds to store
        [Authorize(Roles = RoleConstants.FranchiseHolderRole)]
        public async Task<IActionResult> StoreStockRequests()
        {
            var _user = await _userManager.GetUserAsync(User);
            var stockRequestsList = _context.StockRequest.Include(s => s.Store).Include(o => o.Product).Where(s => s.StoreID == _user.StoreID);
            return View(await stockRequestsList.ToListAsync());         
            
        }

        [Authorize(Roles =RoleConstants.FranchiseHolderRole)]
        public async Task<IActionResult> Create(int StoreID, int ProductID) {

            var _user = await _userManager.GetUserAsync(User);
            // var x = _context.OwnerInventory.Include(s => s.StockLevel);
            // check fanchise holder valid to see own data
            if (StoreID != _user.StoreID) {
                return NotFound();
            }
            // var stockRequest = await _context.StockRequest.Include(s => s.Store).SingleOrDefaultAsync(s => s.StoreID == StoreID);
            //Get Store Name and Product Name in database
            var storeName = _context.Store.SingleOrDefaultAsync(s => s.StoreID == StoreID);
            var productName = _context.Product.SingleOrDefaultAsync(o => o.ProductID == ProductID);

            //Get stock level using StoreID and ProductId
           // var currentStockForSore =  _context.StoreInventory.Where(s => s.StoreID == StoreID );
            //var currentStock =await currentStockForSore.SingleOrDefaultAsync(o => o.ProductID == ProductID);
           
            var currentstockLevel = await GetStockLevel(StoreID, ProductID);
              
            ViewBag.StoreName = storeName.Result.Name;
            ViewBag.ProductName = productName.Result.Name;
            ViewBag.CurrentStock = currentstockLevel;
            return View();
        }

        [Authorize(Roles = RoleConstants.FranchiseHolderRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreID, ProductID, Quantity")] StockRequest stockRequest) {

            if (ModelState.IsValid) {
                _context.Add(stockRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(StoreStockRequests));
            } 
            return View();
        }

        public async Task<IActionResult> ProcessStockRequest(int? id) {
            if (id == null) {
                return NotFound();
            }
            var stockRequestProcess =await _context.StockRequest.Include(s => s.Store).Include(o => o.Product).SingleOrDefaultAsync( r => r.StockRequestID == id);

            //Get stock level using StoreID and ProductId
            // var currentStockForSore = _context.StoreInventory.Where(s => s.StoreID == stockRequestProcess.StoreID);
            //var currentStock = await currentStockForSore.SingleOrDefaultAsync(o => o.ProductID == stockRequestProcess.ProductID);
            //var stockLevel = currentStock.StockLevel;

            var ownerInventoryProduct = await _context.OwnerInventory.SingleOrDefaultAsync(o => o.ProductID == stockRequestProcess.ProductID);

            var ownerStockLevel = ownerInventoryProduct.StockLevel;

            ViewBag.StockLevel = ownerStockLevel;

            ViewBag.Avalability = CheckStockAvalability(ownerStockLevel, stockRequestProcess.Quantity);

            return View(stockRequestProcess);
        }

        public async Task<IActionResult> StockRequestApproved() {


            return RedirectToAction(nameof(Index));
        }

        //Get stock level using StoreID and ProductId
        private async Task<int> GetStockLevel(int storeID, int productID) {          
            
            var currentStockForSore = _context.StoreInventory.Where(s => s.StoreID == storeID);
            var currentStock =await currentStockForSore.SingleOrDefaultAsync(o => o.ProductID == productID);
            var stockLevel = currentStock.StockLevel;
            return stockLevel;
        }

        //Check the stock availability in OwnerInventory
        private bool CheckStockAvalability(int currentStockLevel, int quantityRequest) {
            var avalability = false;
            if (currentStockLevel >= quantityRequest) {
                return avalability = true;
            }

            return avalability;
        }
    }
}