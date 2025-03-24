using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI_PhanTranMinhTam_New.Authorization;
using WebAPI_PhanTranMinhTam_New.Data;
using WebAPI_PhanTranMinhTam_New.Mappings;
using WebAPI_PhanTranMinhTam_New.Reponsitory;
using WebAPI_PhanTranMinhTam_New.Services;
using WebAPI_PhanTranMinhTam_New.Validations;
//using WebAPI_PhanTranMinhTam_GIFT.Vadications;
namespace WebAPI_PhanTranMinhTam_New
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            // Đọc cấu hình từ appsettings.json
            IConfigurationSection jwtSettings = builder.Configuration.GetSection("Jwt");
            string secretKey = builder.Configuration["AppSettings:SecretKey"];
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            // Add services to the container.
            // Configure AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingGift));
            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            // Đăng ký JwtSecurityTokenHandler
            builder.Services.AddSingleton<JwtSecurityTokenHandler>();

            // Đăng ký TokenValidationParameters
            builder.Services.AddSingleton<TokenValidationParameters>(provider =>
            {
                return new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "your_issuer",
                    ValidAudience = "your_audience",
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ClockSkew = TimeSpan.Zero
                };
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("MyDB")));
            builder.Services.AddHangfireServer();

            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));
            builder.Services.AddScoped<IOrderServices, OrderServices>();
            builder.Services.AddScoped<IRankServices, RankServices>();
            builder.Services.AddScoped<ISendGiftServices, SendGiftServices>();
            builder.Services.AddScoped<ICartServices, CartServices>();
            builder.Services.AddScoped<IPermisstionRolwServices, RolePermisstionServices>();
            builder.Services.AddScoped<IPermisstionServices, PermistionServices>();
            builder.Services.AddScoped<IRoleServices, RoleServices>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<IGiftServices, GiftServices>();
            builder.Services.AddScoped<IAuthService, AuthServices>();
            builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            builder.Services.AddScoped<TokenValidation>(provider =>
            {
                IRepositoryWrapper reponsitory = provider.GetRequiredService<IRepositoryWrapper>();
                JwtSecurityTokenHandler tokenHandler = provider.GetRequiredService<JwtSecurityTokenHandler>();
                TokenValidationParameters tokenValidationParameters = provider.GetRequiredService<TokenValidationParameters>();

                return new TokenValidation(reponsitory, tokenHandler, tokenValidationParameters);
            });
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
