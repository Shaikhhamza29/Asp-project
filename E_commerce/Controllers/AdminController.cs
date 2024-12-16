using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace E_commerce.Controllers
{
    [Authorize]  // Ensure only authenticated users can access this page
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the ApplicationDbContext into the constructor
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var products = await _context.Products.ToListAsync();  // Fetch products from the database
            return View(products);  // Pass the list of products to the view
        }

        // GET: /Admin/AddProduct
        public IActionResult AddProduct()
        {
            return View();  // Display the product creation form
        }

        // POST: /Admin/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(string name, string description, decimal price, string imageUrl)
        {
            // Validate the image URL
            if (string.IsNullOrEmpty(imageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Image URL is required.");
                return View();  // Return the view with an error
            }

            // Validate other fields
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Product Name is required.");
            }
            if (string.IsNullOrEmpty(description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }
            if (price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                return View();  // Return the view with validation errors
            }

            // Add the new product to the database
            var newProduct = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl
            };

            _context.Products.Add(newProduct);  // Add the product to the DbSet
            await _context.SaveChangesAsync();  // Save the changes to the database

            // Redirect back to the dashboard to see the updated list
            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/EditProduct/{id}
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();  // Return 404 if product is not found
            }

            return View(product);  // Pass the product to the view for editing
        }

        // POST: /Admin/EditProduct/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, string name, string description, decimal price, string imageUrl)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();  // Return 404 if product is not found
            }

            // Validate the image URL
            if (string.IsNullOrEmpty(imageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Image URL is required.");
            }

            // Validate other fields
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Product Name is required.");
            }
            if (string.IsNullOrEmpty(description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }
            if (price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                return View(product);  // Return the view with validation errors
            }

            // Update the product
            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();  // Save changes to the database

            // Redirect back to the dashboard after updating
            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/DeleteProduct/{id}
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);  // Remove the product from the DbSet
                await _context.SaveChangesAsync();  // Save changes to the database
            }

            // Redirect to the Dashboard after deletion
            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return View(new LoginViewModel());
        }


        // POST: /Admin/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Example admin login (replace with actual validation logic)
                if (model.Username == "admin" && model.Password == "admin")
                {
                    // Create a claims identity and claims principal
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Set authentication cookie with sliding expiration and persistent login
                    await HttpContext.SignInAsync("Cookies", claimsPrincipal, new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,  // If RememberMe is checked, persist the cookie
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)  // Cookie will expire after 7 days
                    });

                    // Redirect to the Dashboard after successful login
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            return View(model);  // Return to the login page with error messages if the login fails
        }



        // GET: /Admin/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");  // Sign out the user
            return RedirectToAction("Login");  // Redirect to the login page
        }
    }
}
