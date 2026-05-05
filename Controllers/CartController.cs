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
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController()
        {
            _db = new ApplicationDbContext();
        }

        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartItems.Select(ci => ci.MenuItem))
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        // GET: /Cart/Index
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var cart = await GetOrCreateCartAsync(userId);

            var vm = new CartViewModel { CartId = cart.Id };
            if (cart.CartItems != null)
            {
                foreach (var ci in cart.CartItems)
                {
                    vm.Items.Add(new CartItemViewModel
                    {
                        CartItemId = ci.Id,
                        MenuItemId = ci.MenuItemId,
                        MenuItemName = ci.MenuItem.Name,
                        UnitPrice = ci.MenuItem.Price,
                        Quantity = ci.Quantity,
                        RestaurantId = 0,
                        RestaurantName = "Habesha Kitchen"
                    });
                }
                if (vm.Items.Any())
                {
                    vm.RestaurantId = 0;
                    vm.RestaurantName = "Habesha Kitchen";
                }
            }
            return View(vm);
        }

        // POST: /Cart/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int menuItemId, int quantity = 1)
        {
            var userId = User.Identity.GetUserId();
            var menuItem = await _db.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId && m.IsAvailable);

            if (menuItem == null)
            {
                TempData["Error"] = "Item not available.";
                return RedirectToAction("Index");
            }

            var cart = await GetOrCreateCartAsync(userId);

            var existing = cart.CartItems?.FirstOrDefault(ci => ci.MenuItemId == menuItemId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    MenuItemId = menuItemId,
                    Quantity = quantity
                });
            }

            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = $"'{menuItem.Name}' added to cart.";
            }
            catch (System.Exception ex)
            {
                string innerHtml = ex.Message;
                if (ex.InnerException != null)
                {
                    innerHtml += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        innerHtml += " | Core: " + ex.InnerException.InnerException.Message;
                    }
                }
                TempData["Error"] = "Database Error: " + innerHtml;
            }
            
            return RedirectToAction("Index", "Home");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = User.Identity.GetUserId();
            var cart = await _db.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return RedirectToAction("Index");

            var item = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
                return new HttpStatusCodeResult(403);

            if (quantity <= 0)
                _db.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItem(int cartItemId)
        {
            var userId = User.Identity.GetUserId();
            var cart = await _db.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return RedirectToAction("Index");

            var item = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // POST: /Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Clear()
        {
            var userId = User.Identity.GetUserId();
            var cart = await _db.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart?.CartItems != null)
            {
                _db.CartItems.RemoveRange(cart.CartItems);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
