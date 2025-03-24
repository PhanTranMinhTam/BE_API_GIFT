using Microsoft.EntityFrameworkCore;

namespace WebAPI_PhanTranMinhTam_New.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Cartitem> Cartitems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Orderitem> Orderitems { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Rolepermission> Rolepermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivitys { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cart
            modelBuilder.Entity<Cart>()
                .HasKey(c => c.IdCart);
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem
            modelBuilder.Entity<Cartitem>()
                .HasKey(c => c.IdCartItem);
            modelBuilder.Entity<Cartitem>()
                .HasIndex(c => new { c.IdCart, c.IdGift }).IsUnique();
            modelBuilder.Entity<Cartitem>()
                .HasOne(c => c.Gifts)
                .WithMany(g => g.Cartitems)
                .HasForeignKey(c => c.IdGift)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cartitem>()
                .HasOne(c => c.Carts)
                .WithMany(c => c.Cart_items)
                .HasForeignKey(c => c.IdCart)
                .OnDelete(DeleteBehavior.Cascade);

            // Gift
            modelBuilder.Entity<Gift>()
                .HasKey(g => g.IdGift);
            modelBuilder.Entity<Gift>()
                .HasMany(g => g.Cartitems)
                .WithOne(c => c.Gifts)
                .HasForeignKey(c => c.IdGift)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Gift>()
                .HasMany(g => g.Orderitem)
                .WithOne(oi => oi.Gifts)
                .HasForeignKey(oi => oi.IdGift)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Gift>()
                .HasOne(g => g.ParentGift)
                .WithMany(g => g.ChildGifts)
                .HasForeignKey(g => g.Idparents)
                .OnDelete(DeleteBehavior.Restrict);

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.IdOrder);

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(o => o.Order_items)
                    .WithOne(oi => oi.Orders)
                    .HasForeignKey(oi => oi.IdOrder)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem
            modelBuilder.Entity<Orderitem>(entity =>
            {
                entity.HasKey(oi => oi.IdOrderItem);

                entity.HasOne(oi => oi.Orders)
                    .WithMany(o => o.Order_items)
                    .HasForeignKey(oi => oi.IdOrder)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Gifts)
                    .WithMany(g => g.Orderitem)
                    .HasForeignKey(oi => oi.IdGift)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.ParentGift)
                    .WithMany(g => g.ChildGifts)
                    .HasForeignKey(oi => oi.IdParent)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(oi => new { oi.IdOrder, oi.IdGift })
                    .IsUnique();
            });

            // UserActivity
            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(ua => ua.Id);

                entity.HasOne(ua => ua.User)
                    .WithMany(u => u.UserActivitys)
                    .HasForeignKey(ua => ua.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.IdUser);

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Carts)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.IdRoles);
            modelBuilder.Entity<Role>()
                .HasMany(r => r.users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Role>()
                .HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.IdRole)
                .OnDelete(DeleteBehavior.Cascade);

            // Permission
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.IdPermission);
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.IdPermission)
                .OnDelete(DeleteBehavior.Cascade);

            // RolePermission
            modelBuilder.Entity<Rolepermission>()
                .HasKey(rp => new { rp.IdRole, rp.IdPermission });
            modelBuilder.Entity<Rolepermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.IdRole);
            modelBuilder.Entity<Rolepermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.IdPermission);
        }
    }
}
