using Autofac;
using DAL;
using DAL.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using System;
using WebAPI.Services;

namespace WebAPI
{
    public class Startup
    {
        private readonly ILogger<Startup> log;
        public Startup(IConfiguration configuration, ILogger<Startup> log)
        {
            Configuration = configuration;
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builderDbContext = new DbContextOptionsBuilder<DBContext>()
                    .UseMySql(Configuration.GetConnectionString("DBConnectionString"),
                    b => b.MigrationsAssembly("DAL"));
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<WebAPI.Services.IUserService, UserService>();
            IHttpContextAccessor httpContextAccessor = null;
            WebAPI.Services.IUserService userService = new UserService(httpContextAccessor);
            using (var context = new DBContext(builderDbContext.Options, userService))
            {
                Policy.Handle<Exception>()
                      .WaitAndRetry(5, retryAttempt => TimeSpan.FromMinutes(1 * retryAttempt), (ex, time) =>
                      {
                          log.LogWarning(ex, ex.Message);
                      })
                      .Execute(() => context.Database.MigrateAsync().GetAwaiter().GetResult());
            }
            services.AddDbContext<DBContext>(options => { options.UseMySql(Configuration.GetConnectionString("DBConnectionString")); });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
           .AddEntityFrameworkStores<DBContext>();
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web API",
                    Version = "v1",
                    Description = "Web API",
                });

            });

            services.AddLogging((logingbuilder) =>
            {
                logingbuilder.AddConsole();

#if (DEBUG)
                logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.debug.config");
#else
                       logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.config");     
#endif

                logingbuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterModule(new ApplicationModel());

            var container = containerBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
