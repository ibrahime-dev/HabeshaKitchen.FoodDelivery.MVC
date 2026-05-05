using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FoodDelivery.Data;
using FoodDelivery.Models.ViewModels;

namespace FoodDelivery.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: /Admin/Index
        public async Task<ActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TotalUsers = await _db.Users.CountAsync(),
                TotalOrders = await _db.Orders.CountAsync(),
                TotalMenuItems = await _db.MenuItems.CountAsync()
            };
            return View(vm);
        }

        // GET: /Admin/Users
        public async Task<ActionResult> Users()
        {
            var users = await _db.Users.ToListAsync();
            var roleStore = new RoleStore<IdentityRole>(_db);

            var vm = new System.Collections.Generic.List<AdminUserViewModel>();
            using (var userManager = new UserManager<Models.ApplicationUser>(new UserStore<Models.ApplicationUser>(_db)))
            {
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user.Id);
                    vm.Add(new AdminUserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = roles.FirstOrDefault() ?? "None",
                        IsActive = user.IsActive
                    });
                }
            }

            return View(vm);
        }

        // POST: /Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleUserStatus(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return HttpNotFound();

            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"User '{user.Email}' has been {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction("Users");
        }

        // GET: /Admin/Orders
        public async Task<ActionResult> Orders()
        {
            var orders = await _db.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var vm = orders.Select(o => new AdminOrderViewModel
            {
                OrderId = o.Id,
                CustomerEmail = o.Customer?.Email ?? "N/A",
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            }).ToList();

            return View(vm);
        }

        // GET: /Admin/CreateOwner
        public ActionResult CreateOwner()
        {
            return View(new CreateOwnerViewModel());
        }

        // POST: /Admin/CreateOwner
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOwner(CreateOwnerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var userManager = new UserManager<Models.ApplicationUser>(new UserStore<Models.ApplicationUser>(_db)))
            {
                var user = new Models.ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id, "Restaurant_Owner");
                    TempData["Success"] = $"Restaurant owner {user.Email} created successfully.";
                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
