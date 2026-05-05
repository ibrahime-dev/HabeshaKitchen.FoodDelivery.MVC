using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FoodDelivery.Data;
using FoodDelivery.Helpers;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;

namespace FoodDelivery.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RestaurantController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: /Restaurant/Index - redirect to single restaurant menu
        public async Task<ActionResult> Index(string search)
        {
            var restaurant = await _db.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.IsActive);

            if (restaurant == null)
                return View("~/Views/Shared/NotFound.cshtml");

            var items = restaurant.MenuItems.Where(m => m.IsAvailable);

            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(m => m.Name.Contains(search) || (m.Description != null && m.Description.Contains(search)));

            var grouped = items
                .GroupBy(m => m.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vm = new RestaurantDetailsViewModel
            {
                Restaurant = restaurant,
                MenuItemsByCategory = grouped,
                SearchTerm = search
            };

            return View("Details", vm);
        }

        // GET: /Restaurant/Details/5
        public async Task<ActionResult> Details(int id, string search)
        {
            var restaurant = await _db.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

            if (restaurant == null)
                return HttpNotFound();

            var items = restaurant.MenuItems.Where(m => m.IsAvailable);

            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(m => m.Name.Contains(search) || (m.Description != null && m.Description.Contains(search)));

            var grouped = items
                .GroupBy(m => m.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vm = new RestaurantDetailsViewModel
            {
                Restaurant = restaurant,
                MenuItemsByCategory = grouped,
                SearchTerm = search
            };

            return View(vm);
        }

        // GET: /Restaurant/Create
        [Authorize(Roles = "Restaurant_Owner")]
        public async Task<ActionResult> Create()
        {
            var userId = User.Identity.GetUserId();
            var existing = await _db.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);
            if (existing != null)
            {
                TempData["Error"] = "You already have a restaurant registered.";
                return RedirectToAction("Edit", new { id = existing.Id });
            }
            return View(new RestaurantCreateEditViewModel());
        }

        // POST: /Restaurant/Create
        [HttpPost]
        [Authorize(Roles = "Restaurant_Owner")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RestaurantCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.Identity.GetUserId();
            var existing = await _db.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);
            if (existing != null)
            {
                TempData["Error"] = "You already have a restaurant registered.";
                return RedirectToAction("Dashboard");
            }

            var restaurant = new Restaurant
            {
                Name = model.Name,
                Description = model.Description,
                CuisineCategory = model.CuisineCategory,
                PhoneNumber = model.PhoneNumber,
                OwnerId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Restaurants.Add(restaurant);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Restaurant created successfully!";
            return RedirectToAction("Dashboard");
        }

        // GET: /Restaurant/Edit/5
        [Authorize(Roles = "Restaurant_Owner")]
        public async Task<ActionResult> Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == id && r.OwnerId == userId);
            if (restaurant == null)
                return new HttpStatusCodeResult(403);

            var vm = new RestaurantCreateEditViewModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                CuisineCategory = restaurant.CuisineCategory,
                PhoneNumber = restaurant.PhoneNumber
            };
            return View(vm);
        }

        // POST: /Restaurant/Edit/5
        [HttpPost]
        [Authorize(Roles = "Restaurant_Owner")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RestaurantCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.Identity.GetUserId();
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == model.Id && r.OwnerId == userId);
            if (restaurant == null)
                return new HttpStatusCodeResult(403);

            restaurant.Name = model.Name;
            restaurant.Description = model.Description;
            restaurant.CuisineCategory = model.CuisineCategory;
            restaurant.PhoneNumber = model.PhoneNumber;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Restaurant updated successfully!";
            return RedirectToAction("Dashboard");
        }

        // GET: /Restaurant/Dashboard
        [Authorize(Roles = "Restaurant_Owner")]
        public async Task<ActionResult> Dashboard(string statusFilter)
        {
            var userId = User.Identity.GetUserId();
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);

            if (restaurant == null)
            {
                TempData["Info"] = "Please create your restaurant first.";
                return RedirectToAction("Create");
            }

            var query = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .Where(o => o.RestaurantId == restaurant.Id);

            if (!string.IsNullOrWhiteSpace(statusFilter) && Enum.TryParse<OrderStatus>(statusFilter, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            var dashboardOrders = orders.Select(o => new DashboardOrderViewModel
            {
                OrderId = o.Id,
                CustomerName = o.Customer?.UserName ?? "Unknown",
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                NextStatus = OrderStatusHelper.GetNextStatus(o.Status)
            }).ToList();

            var vm = new RestaurantDashboardViewModel
            {
                Restaurant = restaurant,
                Orders = dashboardOrders,
                StatusFilter = statusFilter
            };

            return View(vm);
        }

        // POST: /Restaurant/UpdateOrderStatus
        [HttpPost]
        [Authorize(Roles = "Restaurant_Owner")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrderStatus(OrderStatusUpdateViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);
            if (restaurant == null)
                return new HttpStatusCodeResult(403);

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == model.OrderId && o.RestaurantId == restaurant.Id);
            if (order == null)
                return new HttpStatusCodeResult(403);

            if (!OrderStatusHelper.IsValidTransition(order.Status, model.NewStatus))
            {
                TempData["Error"] = $"Cannot transition from {order.Status} to {model.NewStatus}.";
                return RedirectToAction("Dashboard");
            }

            order.Status = model.NewStatus;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Order #{order.Id} status updated to {model.NewStatus}.";
            return RedirectToAction("Dashboard");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
