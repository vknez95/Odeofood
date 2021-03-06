using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OdeToFood.Entities;
using OdeToFood.Services;

namespace OdeToFood
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton((provider) => Configuration);
            services.AddSingleton<IGreeter, Greeter>();
            services.AddScoped<IRestaurantData, MySqlRestaurantData>();
            services.AddDbContext<OdeToFoodDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("OdeToFoodMySql")));
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<OdeToFoodDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment environment,
            IGreeter greeter)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = context => context.Response.WriteAsync("Oops!")
                });
            }

            //app.UseWelcomePage();
            //app.UseRuntimeInfoPage();     // obsolete

            // app.UseDefaultFiles();
            // app.UseStaticFiles();
            app.UseFileServer();

            app.UseNodeModules(environment.ContentRootPath);

            app.UseIdentity();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(ConfigureRoutes);

            // app.Run(async (context) =>
            // {
            //     //throw new System.Exception("Error!");

            //     // var greeting = greeter.GetGreeting();
            //     // await context.Response.WriteAsync(greeting);

            //     await context.Response.WriteAsync("Not found");
            // });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
