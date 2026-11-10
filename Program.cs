using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using QuanLyBanHangThueVN.Data;
using QuanLyBanHangThueVN.Forms;
using QuanLyBanHangThueVN.Services;

namespace QuanLyBanHangThueVN
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Seed database
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DatabaseSeeder.Seed(context);
            }

            var mainForm = ServiceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            
            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            services.AddScoped<TaxService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<SalesService>();
            services.AddScoped<StatisticsService>();

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<DashboardForm>();
            services.AddTransient<ProductForm>();
            services.AddTransient<POSForm>();
            services.AddTransient<ImportForm>();
            services.AddTransient<TaxConfigForm>();
            services.AddTransient<ReportForm>();
        }
    }
}