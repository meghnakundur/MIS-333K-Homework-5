using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kundur_Meghna_HW5.DAL;
using Kundur_Meghna_HW5.Models;
using Microsoft.AspNetCore.Authorization;

namespace Kundur_Meghna_HW5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        [AllowAnonymous]

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a product to view!" });

            }

            Product product = await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return View("Error", new String[] { "That product was not found in the database." });

            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,ProductDescription,Price,ProductType")] Product product, int[] SelectedSupplierIDs)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.AllSuppliers = GetAllSuppliers();
                return View(product);
            }
            _context.Add(product);
            await _context.SaveChangesAsync();
            foreach (int supplierID in SelectedSupplierIDs)
            {
                Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Please specify a products to edit!" });
            }

            Product product = await _context.Products.Include(c => c.Suppliers).FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,ProductDescription,Price,ProductType")] Product product, int[] SelectedSupplierIDs)
        {
            if (id != product.ProductID)
            {
                return View("Error", new String[] { "Please specify a known product to edit!" });
            }
            if (ModelState.IsValid == false) //there is something wrong
            {
                ViewBag.AllSuppliers = GetAllSuppliers(product);
                return View(product);
            }

            try
            {

                Product dbProduct = _context.Products
                    .Include(c => c.Suppliers)
                    .FirstOrDefault(c => c.ProductID == product.ProductID);

                List<Supplier> SuppliersToRemove = new List<Supplier>();

         
                foreach (Supplier supplier in dbProduct.Suppliers)
                {
                    if (SelectedSupplierIDs.Contains(supplier.SupplierID) == false)//this department is not on the new list
                    {
                        SuppliersToRemove.Add(supplier);
                    }
                }


                foreach (Supplier supplier in SuppliersToRemove)
                {
                    dbProduct.Suppliers.Remove(supplier);
                    _context.SaveChanges();
                }

                foreach (int supplierID in SelectedSupplierIDs)
                {
                    if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)//this department is NOT already associated with this course
                    {
                        Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                        dbProduct.Suppliers.Add(dbSupplier);
                        _context.SaveChanges();
                    }
                }

                //update the products' scalar properties
                dbProduct.Price = product.Price;
                dbProduct.ProductName = product.ProductName;
                dbProduct.ProductDescription = product.ProductDescription;
                dbProduct.ProductType = product.ProductType;
                //save the changes
                _context.Products.Update(dbProduct);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an error editing this course.", ex.Message });
            }

            //if code gets this far, everything is okay
            //send the user back to the page with all the products
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        private MultiSelectList GetAllSuppliers()
        {
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName");

            return mslAllSuppliers;
        }

        private MultiSelectList GetAllSuppliers(Product product)
        {

            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            List<Int32> selectedSupplierIDs = new List<Int32>();

            foreach (Supplier associatedSupplier in product.Suppliers)
            {
                selectedSupplierIDs.Add(associatedSupplier.SupplierID);
            }
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

            return mslAllSuppliers;
        }

    }
}