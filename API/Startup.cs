using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//Inject repository and interface to CONTROLLER
//-------------------------------------------------------------------------------------------------
//   There are 3 options:

//services.AddTransient - Instantiate for an individual method the request itself -> SHORT life time

//services.AddSingleton - The repository will be created the first time we use it (when the application start)
//                        the method goes tho the CONTROLLER and create a new instance of the repository.
//                        It will NEVER BE DESTROYED until the appication stop -> TOO LONG

//USING service.AddScoped to inject the repository and interface into CONTROLLER because
//       the repository will be create when the HTTP request comes into our API -> create new instance of the CONTROLLER
//       the CONTROLLER sees that it needs a repository -> it create the instance of the repository. When the request complete
//       it disposes of both the CONTROLLER and the repository.
//       => We don't need to worry about disposing of the resources when a request comes in.
//--------------------------------------------------------------------------------------------------
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddControllers();
            services.AddDbContext<StoreContext>(x => 
                x.UseSqlite(_config.GetConnectionString("DefaultConnection")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
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
