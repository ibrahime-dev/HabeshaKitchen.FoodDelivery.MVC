using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FoodDelivery.Data;

namespace FoodDelivery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController()
        {
            _db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            if (Request.IsAuthenticated && User.IsInRole("Restaurant_Owner"))
                return RedirectToAction("Dashboard", "Restaurant");

            var menuItems = _db.MenuItems.Where(m => m.IsAvailable).OrderBy(m => m.Category).ThenBy(m => m.Name).ToList();
            ViewBag.RestaurantName = "Habesha Kitchen";
            return View(menuItems);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View(new FoodDelivery.Models.ViewModels.ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(FoodDelivery.Models.ViewModels.ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // In a real app, send email here
                TempData["Success"] = "Thank you! Your message has been sent successfully.";
                return RedirectToAction("Contact");
            }
            return View(model);
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
