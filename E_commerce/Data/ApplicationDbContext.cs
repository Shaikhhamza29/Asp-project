using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_commerce.Models;  // Correct namespace where ApplicationUser is defined

namespace E_commerce.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add your DbSet properties for other entities (not related to Identity)
        public DbSet<Product> Products { get; set; }
        // Other entities...
    }


}
