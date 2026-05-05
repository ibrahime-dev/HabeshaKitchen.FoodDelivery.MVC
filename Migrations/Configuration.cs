using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FoodDelivery.Data;
using FoodDelivery.Models;

namespace FoodDelivery.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "FoodDelivery.Data.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string[] roles = { "Customer", "Restaurant_Owner", "Administrator" };
            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                    roleManager.Create(new IdentityRole(role));
            }

            // Seed default admin
            const string adminEmail = "admin@fooddelivery.com";
            if (userManager.FindByEmail(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    IsActive = true,
                    EmailConfirmed = true
                };
                userManager.Create(admin, "Admin@123456");
                userManager.AddToRole(admin.Id, "Administrator");
            }

            // Seed default restaurant owner
            const string ownerEmail = "owner@fooddelivery.com";
            if (userManager.FindByEmail(ownerEmail) == null)
            {
                var owner = new ApplicationUser
                {
                    UserName = ownerEmail,
                    Email = ownerEmail,
                    FullName = "Restaurant Owner",
                    IsActive = true,
                    EmailConfirmed = true
                };
                userManager.Create(owner, "Owner@123456");
                userManager.AddToRole(owner.Id, "Restaurant_Owner");
            }

            // Seed Menu - only if empty, never delete existing data
            if (!context.MenuItems.Any())
            {
                var ownerUser2 = userManager.FindByEmail(ownerEmail);
                var restaurant = context.Restaurants.FirstOrDefault(r => r.OwnerId == ownerUser2.Id);
                if (restaurant == null)
                {
                    restaurant = new Restaurant
                    {
                        Name = "Habesha Kitchen",
                        Description = "Authentic Ethiopian cuisine delivered to your door.",
                        CuisineCategory = "Ethiopian",
                        PhoneNumber = "0501234567",
                        OwnerId = ownerUser2.Id,
                        IsActive = true,
                        CreatedAt = System.DateTime.UtcNow
                    };
                    context.Restaurants.Add(restaurant);
                    context.SaveChanges();
                }

                int rid = restaurant.Id;
                context.MenuItems.AddRange(new[]
                {
                    new MenuItem { Name = "Extra Injera", Description = "Fresh soft injera", Price = 30.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid, ImageUrl = "https://thumbs.dreamstime.com/b/vibrant-ethiopian-feast-served-traditional-mesob-injera-assortment-dips-colorful-abundant-platter-featuring-410459003.jpg" },
                    new MenuItem { Name = "Doro Wot", Description = "Spicy chicken stew with egg", Price = 250.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid, ImageUrl = "https://gypsyplate.com/wp-content/uploads/2024/10/doro-wat_square.jpg" },
                    new MenuItem { Name = "Sega Wot", Description = "Spicy beef stew", Price = 220.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Shiro Wot", Description = "Chickpea stew (vegan)", Price = 180.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Misir Wot", Description = "Lentil stew", Price = 170.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Gomen", Description = "Collard greens", Price = 140.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Atkilt Wot", Description = "Cabbage, carrots, and potatoes", Price = 150.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Special", Description = "Special Ethiopian combination platter with assorted wots and injera", Price = 600.00m, Category = "Injera & Wot", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Beef Tibs", Description = "Fried beef with onions, peppers, and spices", Price = 260.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Kitfo", Description = "Spiced raw or lightly cooked minced beef with butter", Price = 280.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Doro Tibs", Description = "Fried chicken pieces cooked with onions and spices", Price = 240.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Lamb Tibs", Description = "Tender lamb sautéed with garlic, peppers, and herbs", Price = 290.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Lamb Tibs Awaze", Description = "Awaze and Injera", Price = 6000.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Gored Gored", Description = "Cubed raw beef served fresh with spicy butter and mitmita", Price = 300.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Dulet", Description = "Mixed liver, tripe, and minced meat cooked with spices", Price = 270.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Lebleb Tibs", Description = "Soft, lightly sautéed beef", Price = 250.00m, Category = "Tibs", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Ethiopian Coffee (Buna)", Description = "Traditional coffee", Price = 50.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Macchiato", Description = "Macchiato", Price = 60.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Tea (Shai)", Description = "Tea", Price = 40.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Soft Drinks", Description = "Coca-Cola, Fanta, Sprite", Price = 40.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Fresh Juice", Description = "Mango, Avocado, Papaya", Price = 80.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Avocado Juice", Description = "Rich, thick fresh blended avocado", Price = 90.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Mango Juice", Description = "Freshly blended sweet mango", Price = 80.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                    new MenuItem { Name = "Spris Juice", Description = "Layers of fresh avocado, mango, and papaya", Price = 100.00m, Category = "Drinks", IsAvailable = true, RestaurantId = rid },
                });
                context.SaveChanges();
            }
        }
    }
}
