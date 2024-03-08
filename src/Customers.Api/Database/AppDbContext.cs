using Customers.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api.Database;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
            
    }
}
