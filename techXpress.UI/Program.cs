using techXpress.Services.Abstraction;
using techXpress.Services.Managers;
using techXpress.Services.Services;
using techXpress.DataAccess.Abstraction;
using techXpress.DataAccess.Data;
using techXpress.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using techXpress.DataAccess.Entities;

namespace techXpress.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add Services to container
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
                optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Devlopment_DB"))
                    .LogTo(Console.WriteLine , LogLevel.Information)
                    .EnableSensitiveDataLogging()
            );
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IProductManager, ProductManager>();
            builder.Services.AddTransient<IFilesService, FilesService>(); 
            builder.Services.AddTransient<IOrderManger, OrderManger>(); 

            builder.Services.AddScoped<ICategoryManager, CategoryManager>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.MaxAge = TimeSpan.FromDays(7); 
            });
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;

            }).AddEntityFrameworkStores<AppDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });

            #endregion

            #region Configure the HTTP request pipeline.
            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run(); 
            #endregion
        }
    }
}
