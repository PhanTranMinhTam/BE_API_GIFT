using Microsoft.EntityFrameworkCore.Storage;
using WebAPI_PhanTranMinhTam_New.Data;

namespace WebAPI_PhanTranMinhTam_New.Reponsitory
{
    public interface IGiftRepository : IRepositoryBase<Gift> { }
    public interface IUserRepository : IRepositoryBase<User> { }
    public interface IAuthReponsitory : IRepositoryBase<Models.AuthDTO> { }
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken> { }
    public interface ICartRepository : IRepositoryBase<Cart> { }
    public interface ICartItemRepository : IRepositoryBase<Cartitem> { }
    public interface IOrderRepository : IRepositoryBase<Order> { }
    public interface IOrderItemRepository : IRepositoryBase<Orderitem> { }
    public interface IRolePermisstionRepository : IRepositoryBase<Rolepermission> { }
    public interface IRoleRepository : IRepositoryBase<Role> { }
    public interface IPermisstionRepository : IRepositoryBase<Permission> { }
    public interface IUserActivityRepository : IRepositoryBase<UserActivity> { }
    public interface IRepositoryWrapper
    {
        IGiftRepository Gift { get; }
        IUserRepository User { get; }
        IAuthReponsitory Auth { get; }
        ICartRepository Cart { get; }
        ICartItemRepository CartItem { get; }
        IOrderRepository Order { get; }
        IOrderItemRepository OrderItem { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IRolePermisstionRepository RolePermisstion { get; }
        IRoleRepository Role { get; }
        IPermisstionRepository Permisstion { get; }
        IUserActivityRepository UserActivity { get; }
        void Save();
        Task SaveAsync();
        IDbContextTransaction Transaction();
    }

    public class GiftRepository : ReponsitoryBase<Gift>, IGiftRepository
    {
        public GiftRepository(MyDbContext context) : base(context) { }
    }
    public class UserRepository : ReponsitoryBase<User>, IUserRepository
    {
        public UserRepository(MyDbContext context) : base(context) { }
    }
    public class AuthReponsitory : ReponsitoryBase<Models.AuthDTO>, IAuthReponsitory
    {
        public AuthReponsitory(MyDbContext context) : base(context) { }
    }
    public class RefreshTokenRepository : ReponsitoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(MyDbContext context) : base(context) { }
    }

    public class CartRepository : ReponsitoryBase<Cart>, ICartRepository
    {
        public CartRepository(MyDbContext context) : base(context) { }
    }
    public class CartItemRepository : ReponsitoryBase<Cartitem>, ICartItemRepository
    {
        public CartItemRepository(MyDbContext context) : base(context) { }
    }
    public class OrderRepository : ReponsitoryBase<Order>, IOrderRepository
    {
        public OrderRepository(MyDbContext context) : base(context) { }
    }
    public class OrderItemRepository : ReponsitoryBase<Orderitem>, IOrderItemRepository
    {
        public OrderItemRepository(MyDbContext context) : base(context) { }
    }
    public class RolePermisstionRepository : ReponsitoryBase<Rolepermission>, IRolePermisstionRepository
    {
        public RolePermisstionRepository(MyDbContext context) : base(context) { }
    }
    public class RoleRepository : ReponsitoryBase<Role>, IRoleRepository
    {
        public RoleRepository(MyDbContext context) : base(context) { }
    }
    public class PermisstionRepository : ReponsitoryBase<Permission>, IPermisstionRepository
    {
        public PermisstionRepository(MyDbContext context) : base(context) { }
    }
    public class UserActivityRepository : ReponsitoryBase<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(MyDbContext context) : base(context) { }
    }
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IGiftRepository gift;
        private IUserRepository user;
        private IAuthReponsitory AuthRepository;
        private IRefreshTokenRepository RefreshRepository;
        private ICartRepository CartRepository;
        private ICartItemRepository CartItemRepository;
        private IOrderRepository OrderRepository;
        private IOrderItemRepository OrderItemRepository;
        private IRolePermisstionRepository RolePermisstionRepository;
        private IRoleRepository RoleRepository;
        private IPermisstionRepository PermisstionRepository;
        private IUserActivityRepository UserActivityRepository;
        private readonly MyDbContext context;

        public RepositoryWrapper(MyDbContext context)
        {
            this.context = context;
        }

        public IGiftRepository Gift => gift ??= new GiftRepository(context);
        public IUserActivityRepository UserActivity => UserActivityRepository ??= new UserActivityRepository(context);
        public IUserRepository User => user ??= new UserRepository(context);
        public IAuthReponsitory Auth => AuthRepository ??= new AuthReponsitory(context);
        public IRefreshTokenRepository RefreshToken => RefreshRepository ??= new RefreshTokenRepository(context);
        public ICartRepository Cart => CartRepository ??= new CartRepository(context);
        public ICartItemRepository CartItem => CartItemRepository ??= new CartItemRepository(context);
        public IOrderRepository Order => OrderRepository ??= new OrderRepository(context);
        public IOrderItemRepository OrderItem => OrderItemRepository ??= new OrderItemRepository(context);

        public IRolePermisstionRepository RolePermisstion => RolePermisstionRepository ??= new RolePermisstionRepository(context);
        public IRoleRepository Role => RoleRepository ??= new RoleRepository(context);
        public IPermisstionRepository Permisstion => PermisstionRepository ??= new PermisstionRepository(context);
        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public IDbContextTransaction Transaction()
        {
            return context.Database.BeginTransaction();
        }
    }
}
