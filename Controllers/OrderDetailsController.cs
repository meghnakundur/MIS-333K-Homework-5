using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kundur_Meghna_HW5.DAL;
using Kundur_Meghna_HW5.Models;

namespace Kundur_Meghna_HW5.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new String[] { "Please specify a registration to view!" });
            }

            List<OrderDetail> rds = _context.OrderDetails
                                          .Include(rd => rd.Product)
                                          .Where(rd => rd.Order.OrderID == orderID)
                                          .ToList();

            return View(rds);
        }

        // GET: OrderDetails/Details/5


        // GET: OrderDetails/Create
        public IActionResult Create(int orderID)
        {
            OrderDetail od = new OrderDetail();

            Order dbOrder = _context.Orders.Find(orderID);

            od.Order = dbOrder;

            //populate the ViewBag with a list of existing products
            ViewBag.AllProducts = GetAllProducts();

            return View(od);
        }


        // POST: OrderDetails/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail, int SelectedProduct)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.AllProducts = GetAllProducts();
                return View(orderDetail);
            }
            //find the product to be associated with this order
            Product dbProduct = _context.Products.Find(SelectedProduct);

            orderDetail.Product = dbProduct;
            //find
            Order dbOrder = _context.Orders.Find(orderDetail.Order.OrderID);

            orderDetail.Order = dbOrder;
            orderDetail.Price = dbProduct.Price;
            orderDetail.ExtendedPrice = orderDetail.Quantity * orderDetail.Price;

            //add to database
            _context.Add(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order detail to edit!" });
            }

            OrderDetail orderDetail = await _context.OrderDetails
                                                   .Include(od => od.Product)
                                                   .Include(od => od.Order)
                                                   .FirstOrDefaultAsync(od => od.OrderDetailID == id);
            if (orderDetail == null)
                if (orderDetail == null)
                {
                    return View("Error", new String[] { "This order detail was not found" });
                }
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,Quantity,Price,ExtendedPrice")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return View("Error", new String[] { "There was a problem editing this record. Try again!" });
            }

            if (ModelState.IsValid == false)
            {
                return View(orderDetail);
            }
            OrderDetail dbRD;
            //if code gets this far, update the record
            try
            {
                dbRD = _context.OrderDetails
                      .Include(rd => rd.Product)
                      .Include(rd => rd.Order)
                      .FirstOrDefault(rd => rd.OrderDetailID == orderDetail.OrderDetailID);

                //update the scalar properties
                dbRD.Quantity = orderDetail.Quantity;
                dbRD.Price = dbRD.Product.Price;
                dbRD.ExtendedPrice = dbRD.Quantity * dbRD.Price;

                //save changes
                _context.Update(dbRD);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem editing this record", ex.Message });
            }

            return RedirectToAction("Details", "Orders", new { id = dbRD.Order.OrderID });
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderDetail orderDetail = await _context.OrderDetails.Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = await _context.OrderDetails.Include(od => od.Order).FirstOrDefaultAsync(od => od.OrderDetailID == id);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });

        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }

        private SelectList GetAllProducts()
        {
            List<Product> allProducts = _context.Products.ToList();

            SelectList slAllProducts = new SelectList(allProducts, nameof(Product.ProductID), nameof(Product.ProductName));

            return slAllProducts;
        }
    }
}