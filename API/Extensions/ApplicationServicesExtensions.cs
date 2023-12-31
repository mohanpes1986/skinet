using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services,IConfiguration config)
        {
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<StoreContext>(opt=>{
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

            services.AddScoped<IProductRepository,ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionsContext =>
            {
                var errors=actionsContext.ModelState
                .Where(e=>e.Value.Errors.Count>0)
                .SelectMany(x=>x.Value.Errors)
                .Select(x=>x.ErrorMessage).ToArray();

                var errorResponse=new ApiValidationErrorResponse
                    {
                        Errors=errors
                    };
                return new BadRequestObjectResult(errorResponse);
            };
        });
    return services;
        }
    }
}