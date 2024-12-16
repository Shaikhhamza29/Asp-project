using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Controllers
{
    public class CartController : Controller
    {
        // GET: /cart
        public IActionResult cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // POST: /cart/add
        [HttpPost]
        public IActionResult Add(int productId, string productName, decimal productPrice)
        {
            // Retrieve the cart from session, or create a new list if it's not available
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Check if the product is already in the cart
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem == null)
            {
                // Add new item to the cart
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    ProductPrice = productPrice,
                    Quantity = 1
                });
            }
            else
            {
                // Increase quantity of the existing item
                existingItem.Quantity++;
            }

            // Save the updated cart back to session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Return a JSON response with a success message or updated cart data
            return Json(new { success = true, message = "Product added to cart successfully!" });
        }

    }

    public class CartItem
    {
        public int Id { get; set; } // Unique ID for each cart item
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; } // Assuming this is unique for each product
    }

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
