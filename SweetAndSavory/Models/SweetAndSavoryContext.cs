using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SweetAndSavory.Models
{
  public class SweetAndSavoryContext : IdentityDbContext<ApplicationUser>
  {
    public DbSet<Sweet> Sweets { get; set; }
    public DbSet<Savory> Savories { get; set; }
    public DbSet<SweetSavory> SweetsSavories { get; set; }
    public SweetAndSavoryContext(DbContextOptions options) : base(options) { }
  }
}