using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Entities;

namespace ProductManagement.Infrastructure;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions<DataContext> options): base(options)
  {
  }

  public DbSet<Product> Products { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);


  }
}
