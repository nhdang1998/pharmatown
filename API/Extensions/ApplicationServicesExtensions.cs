using System.Linq;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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

            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));

            //Customer basket function
            services.AddScoped<IBasketRepository, BasketRepository>();

            services.Configure<ApiBehaviorOptions>(options => 
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                    
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };

                return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
    }
}