using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FoodDelivery.Data;
using FoodDelivery.Models;
using FoodDelivery.Models.ViewModels;

namespace FoodDelivery.Controllers
{
    [Authorize(Roles = "Restaurant_Owner")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MenuController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: /Menu/Index
        public async Task<ActionResult> Index()
        {
            var items = await _db.MenuItems
                .OrderBy(m => m.Category).ThenBy(m => m.Name)
                .ToListAsync();

            ViewBag.RestaurantName = "Habesha Kitchen";
            return View(items);
        }

        // GET: /Menu/Create
        public ActionResult Create()
        {
            return View(new MenuItemCreateEditViewModel());
        }

        // POST: /Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MenuItemCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var restaurant = await _db.Restaurants.FirstOrDefaultAsync();
            if (restaurant == null)
            {
                ModelState.AddModelError("", "You must create a Restaurant profile first before adding menu items.");
                return View(model);
            }

            var item = new MenuItem
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                IsAvailable = model.IsAvailable,
                RestaurantId = restaurant.Id,
                ImageUrl = SaveImage(model.ImageFile)
            };

            _db.MenuItems.Add(item);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Menu item added successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Menu/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
                return HttpNotFound();

            var vm = new MenuItemCreateEditViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                IsAvailable = item.IsAvailable,
                ImageUrl = item.ImageUrl
            };
            return View(vm);
        }

        // POST: /Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MenuItemCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == model.Id);
            if (item == null)
                return HttpNotFound();

            item.Name = model.Name;
            item.Description = model.Description;
            item.Price = model.Price;
            item.Category = model.Category;
            item.IsAvailable = model.IsAvailable;

            var newImage = SaveImage(model.ImageFile);
            if (newImage != null)
                item.ImageUrl = newImage;
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                item.ImageUrl = model.ImageUrl;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Menu item updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _db.MenuItems
                .Include(m => m.OrderItems)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
                return HttpNotFound();

            if (item.OrderItems != null && item.OrderItems.Any())
            {
                item.IsAvailable = false;
                await _db.SaveChangesAsync();
                TempData["Info"] = "Item is referenced by existing orders and has been marked as unavailable instead of deleted.";
            }
            else
            {
                _db.MenuItems.Remove(item);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Menu item deleted.";
            }

            return RedirectToAction("Index");
        }

        // POST: /Menu/ToggleAvailability/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleAvailability(int id)
        {
            var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
                return HttpNotFound();

            item.IsAvailable = !item.IsAvailable;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Item '{item.Name}' is now {(item.IsAvailable ? "available" : "unavailable")}.";
            return RedirectToAction("Index");
        }

        private string SaveImage(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext))
                return null;

            var folder = Server.MapPath("~/Content/images/menu/");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Path.GetFileNameWithoutExtension(file.FileName)
                           + "_" + System.Guid.NewGuid().ToString("N").Substring(0, 8) + ext;
            file.SaveAs(Path.Combine(folder, fileName));
            return "/Content/images/menu/" + fileName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
