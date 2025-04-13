using Lapka.API.DAL.Models;
using Lapka.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lapka.DAL.DbContext
{
    public class LapkaDbContext : IdentityDbContext<ApplicationUser, Role, string,
                                                              IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
                                                              IdentityRoleClaim<string>, IdentityUserToken<string>>//IdentityDbContext<ApplicationUser, Role, string>
    {
        public LapkaDbContext(DbContextOptions<LapkaDbContext> options)
            : base(options)
        { }
        public LapkaDbContext()
        { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<UserFavorites> UserFavoritesAnimals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Animal>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }
    }
}

