using E_commerce.Data;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IActionResult> Index()
    {
        // Ensure the context and Products table are accessible
        if (_context == null)
        {
            throw new InvalidOperationException("DbContext is not initialized.");
        }

        var products = _context.Products?.ToList();

        if (products == null)
        {
            throw new InvalidOperationException("Products table is empty or not accessible.");
        }

        return View(products);
    }

    public IActionResult about()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
 
}
