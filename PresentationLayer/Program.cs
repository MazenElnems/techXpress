using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add Services to container
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Devlopment_DB")));

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductManager, ProductManager>();
            builder.Services.AddTransient<IFilesService, FilesService>(); 
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
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
