using Formanez_Bringcola.Models;

using Microsoft.EntityFrameworkCore;

namespace Formanez_Bringcola.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
    }
}