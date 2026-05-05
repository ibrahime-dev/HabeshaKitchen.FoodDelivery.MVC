using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;

namespace FoodDelivery.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: /Order/Checkout
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Checkout()
        {
            var userId = User.Identity.GetUserId();
            var cart = await _db.Carts
                .Include(c => c.CartItems.Select(ci => ci.MenuItem))
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var vm = new CheckoutViewModel
            {
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    MenuItemId = ci.MenuItemId,
                    MenuItemName = ci.MenuItem.Name,
                    UnitPrice = ci.MenuItem.Price,
                    Quantity = ci.Quantity
                }).ToList(),
                Total = cart.CartItems.Sum(ci => ci.MenuItem.Price * ci.Quantity)
            };

            return View(vm);
        }

        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var cart = await _db.Carts
                .Include(c => c.CartItems.Select(ci => ci.MenuItem))
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    MenuItemId = ci.MenuItemId,
                    MenuItemName = ci.MenuItem.Name,
                    UnitPrice = ci.MenuItem.Price,
                    Quantity = ci.Quantity
                }).ToList();
                model.Total = cart.CartItems.Sum(ci => ci.MenuItem.Price * ci.Quantity);
                return View(model);
            }

            var restaurantId = cart.CartItems.First().MenuItem.RestaurantId;
            var total = cart.CartItems.Sum(ci => ci.MenuItem.Price * ci.Quantity);

            var order = new Order
            {
                CustomerId = userId,
                RestaurantId = restaurantId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = total,
                Street = model.Street,
                City = model.City,
                PostalCode = model.PostalCode,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    MenuItemId = ci.MenuItemId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.MenuItem.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _db.Payments.Add(new Payment
            {
                OrderId = order.Id,
                Status = PaymentStatus.Pending,
                Method = model.PaymentMethod ?? "Telebirr",
                TxRef = model.TransactionRef,
                ProcessedAt = DateTime.UtcNow
            });

            foreach (var item in cart.CartItems.ToList())
                _db.CartItems.Remove(item);

            await _db.SaveChangesAsync();
            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // GET: /Order/Confirmation/5
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Confirmation(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = await _db.Orders
                .Include(o => o.OrderItems.Select(oi => oi.MenuItem))
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            if (order.CustomerId != userId)
                return new HttpStatusCodeResult(403);

            var vm = new OrderConfirmationViewModel
            {
                OrderId = order.Id,
                RestaurantName = "Habesha Kitchen",
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Street = order.Street,
                City = order.City,
                PostalCode = order.PostalCode,
                Status = order.Status,
                PaymentStatus = order.Payment?.Status.ToString() ?? "N/A",
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return View(vm);
        }

        // GET: /Order/History
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> History()
        {
            var userId = User.Identity.GetUserId();
            var orders = await _db.Orders
                .Include(o => o.OrderItems.Select(oi => oi.MenuItem))
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var vm = orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                RestaurantName = "Habesha Kitchen",
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToList();

            return View(vm);
        }

        // GET: /Order/Details/5
        [Authorize(Roles = "Customer,Restaurant_Owner")]
        public async Task<ActionResult> Details(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems.Select(oi => oi.MenuItem))
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            if (User.IsInRole("Customer") && order.CustomerId != userId)
                return new HttpStatusCodeResult(403);

            var vm = new OrderConfirmationViewModel
            {
                OrderId = order.Id,
                RestaurantName = "Habesha Kitchen",
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Street = order.Street,
                City = order.City,
                PostalCode = order.PostalCode,
                Status = order.Status,
                PaymentStatus = order.Payment?.Status.ToString() ?? "N/A",
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemName = oi.MenuItem?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return View(vm);
        }

        // POST: /Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> Cancel(int id)
        {
            var userId = User.Identity.GetUserId();
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            if (order.CustomerId != userId)
                return new HttpStatusCodeResult(403);

            if (order.Status == OrderStatus.Delivered || 
                order.Status == OrderStatus.Cancelled ||
                order.Status == OrderStatus.Rejected)
            {
                TempData["Error"] = "Order cannot be cancelled at this stage.";
                return RedirectToAction("Details", new { id });
            }

            order.Status = OrderStatus.Cancelled;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Order #{id} has been cancelled.";
            return RedirectToAction("History");
        }

        // --- Owner Dashboard Methods --- //

        [Authorize(Roles = "Restaurant_Owner")]
        public async Task<ActionResult> OwnerDashboard()
        {
            var orders = await _db.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Load payments separately and attach to orders
            var orderIds = orders.Select(o => o.Id).ToList();
            var payments = await _db.Payments
                .Where(p => orderIds.Contains(p.OrderId))
                .ToListAsync();

            foreach (var order in orders)
            {
                var payment = payments.FirstOrDefault(p => p.OrderId == order.Id);
                if (payment != null)
                    order.Payment = payment;
            }

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Restaurant_Owner")]
        public async Task<ActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return HttpNotFound();

            order.Status = newStatus;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Order #{id} status updated to {newStatus}.";
            return RedirectToAction("OwnerDashboard");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
