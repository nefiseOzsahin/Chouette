using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chouette.Models
{
    public class ChouetteIdentityContext:IdentityDbContext<AppUser,AppRole,int>
    {

        public ChouetteIdentityContext(DbContextOptions<ChouetteIdentityContext> options):base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Season> Seasons { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Score> Scores { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         

            modelBuilder.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);
      
            base.OnModelCreating(modelBuilder);


        }
    }
}
