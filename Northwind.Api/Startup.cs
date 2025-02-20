using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Northwind.Api.Models;
using Northwind.Core.Builders;
using Northwind.Core.Extensions;
using Northwind.Core.Models;

namespace Northwind.Api
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Northwind.Api", Version = "v1" });
            });
            services.AddNorthwind();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseRouting();

            var client = new MongoClient(Configuration.GetConnectionString("MDB"));
            var database = client.GetDatabase("Northwind");
            var collection = database.GetCollection<TestClass>("Northwind");

            GroupBuilder<TestClass, int>.New(
                name: nameof(TestClass),
                client: client,
                database: database,
                collection: collection,
                options: new NorthwindOptions { UseTransactions = true, ReturnDocumentState = true }
            )
            .HasPrimaryKey(x => x.Id)
            .WithCustomKeyFieldName("myId")
            .WithIndex(x => x.Name, unique: false)
            .Build();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind.Api v1"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
