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

        [Authorize(Roles =RoleConstants.OwnerRole)]
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
            
            // check fanchise holder valid to see own data
            if (StoreID != _user.StoreID) {
                return NotFound();
            }            
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
           
           // storeID = stockRequestProcess.StoreID;
           // productID = stockRequestProcess.ProductID;
            //Get owner stock level using ProductId     
            var ownerStockLevel = await GetOwnerStockLevel(stockRequestProcess.ProductID);
            var stockRequestQuantity = stockRequestProcess.Quantity;
           // storeStockLevel =await GetStockLevel(storeID, productID);
            ViewBag.StockLevel = ownerStockLevel;
            ViewBag.Avalability = CheckStockAvalability(ownerStockLevel, stockRequestQuantity);

           
            return View(stockRequestProcess);
        }

        public async Task<IActionResult> StockRequestApproved(int? id) {

            var stockRequestProcess = await _context.StockRequest.Include(s => s.Store).Include(o => o.Product).SingleOrDefaultAsync(r => r.StockRequestID == id);
            var stockRequestProductID = stockRequestProcess.ProductID;
            var stockRequestStoreID = stockRequestProcess.StoreID;
            var ownerInventoryProduct = await _context.OwnerInventory.SingleOrDefaultAsync(o => o.ProductID == stockRequestProductID);

            var ownerStockLevel = ownerInventoryProduct.StockLevel;
            var storeStockLevel = await GetStockLevel(stockRequestProcess.StoreID, stockRequestProcess.ProductID);
            var stockRequestQ = stockRequestProcess.Quantity;

            if (CheckStockAvalability(ownerStockLevel,stockRequestQ)) {               

                var storeFilterResults = _context.StoreInventory.Where(s => s.StoreID == stockRequestStoreID);
                var storeProductResult = await storeFilterResults.SingleOrDefaultAsync(o => o.ProductID == stockRequestProductID);

                if (ownerInventoryProduct != null) {
                    ownerInventoryProduct.StockLevel = (ownerStockLevel - stockRequestQ);
                    storeProductResult.StockLevel = (storeStockLevel + stockRequestQ);
                    _context.StockRequest.Remove(stockRequestProcess);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateNewItemRequest() {
            var _user = await _userManager.GetUserAsync(User);
            var ownerProductList =await _context.OwnerInventory.ToListAsync();
            var storeProductList =  _context.StoreInventory.Include(o => o.Product).Where(s => s.StoreID == _user.StoreID);

            var newProductsList = ownerProductList.RemoveAll(x => !storeProductList.Any(y => y.ProductID == x.ProductID));

            ViewBag.NewList = newProductsList;
            return View();
        }

        //Get stock level using StoreID and ProductId
        private async Task<int> GetStockLevel(int storeID, int productID) {          
            
            var currentStockForSore = _context.StoreInventory.Where(s => s.StoreID == storeID);
            var currentStock =await currentStockForSore.SingleOrDefaultAsync(o => o.ProductID == productID);
            var stockLevel = currentStock.StockLevel;
            return stockLevel;
        }

        //Check the stock availability in OwnerInventory
        private bool CheckStockAvalability(int ownerCurrentStockLevel, int quantityRequest) {
            var avalability = false;
            if (ownerCurrentStockLevel >= quantityRequest) {
                return avalability = true;
            }

            return avalability;
        }

        //Return owner stock level for given productID
        private async Task<int> GetOwnerStockLevel(int productID) {
            var ownerInventory = await _context.OwnerInventory.SingleOrDefaultAsync(o => o.ProductID == productID);
            return ownerInventory.StockLevel;
        }


        
    }
}