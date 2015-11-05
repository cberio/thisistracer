using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace thisistracer.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // How to modify User Table
        // 1. Add Properties (public type aaa{get; set;})
        // 2. In package manage console type add-migration 'description' (generate file in Migration foler)
        // 3. Update-database -Scripts shows which is changed.
        // 4. run script or type Update-database in package manage console

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("TRC_User").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<IdentityUserRole>().ToTable("TRC_UserRole");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("TRC_UserLogin");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("TRC_UserClaim").Property(p => p.Id).HasColumnName("UserClaim");
            modelBuilder.Entity<IdentityRole>().ToTable("TRC_Role").Property(p => p.Id).HasColumnName("RoleId");
        }

        public DbSet<Album> Albums { get; set; }
    }
}