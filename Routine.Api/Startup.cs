using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Routine.Api.Data;
using Routine.Api.Services;

namespace Routine.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpCacheHeaders(expires =>
            {
                expires.MaxAge = 60;
                expires.CacheLocation = CacheLocation.Private;
            }, validation =>
            {
                validation.MustRevalidate = true;
            });
            //缓存
            services.AddResponseCaching();

            services.AddControllers(setup =>
            {
                //添加xml格式数据
                setup.ReturnHttpNotAcceptable = true;
                //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                //setup.OutputFormatters.Insert(0,new XmlDataContractSerializerOutputFormatter());
                setup.CacheProfiles.Add("120sCacheProfile",new CacheProfile
                {
                    Duration = 120
                });
            })
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.ContractResolver=
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters();
            //支持格式更多 AddXmlDataContractSerializerFormatters

            services.Configure<MvcOptions>(config =>
            {
                var newtonSoftJsonOutputFormatter =
                    config.OutputFormatters.OfType<NewtonsoftJsonInputFormatter>()?.FirstOrDefault();

                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
            });
            //autoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddDbContext<RoutineDbContext>(options =>
            {
                options.UseSqlite("Data Source=routine.db");
                //options.UseSqlServer("server=.;database=RoutineDb;uid=sa;pwd=123;");
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }

            app.UseCors();

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
