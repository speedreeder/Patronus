using Patronus.DAL;
using Microsoft.EntityFrameworkCore;
using Patronus.API.Services;

namespace Patronus.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<PatronusContext>(options =>
             options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PatronusContext;Trusted_Connection=True;MultipleActiveResultSets=true"));


            builder.Services.AddScoped<IContactService, ContactService>();
            //builder.Services.Scan(scan => scan
            //    .FromCallingAssembly()
            //    .AddClasses(true)
            //    .AsImplementedInterfaces()
            //    .WithScopedLifetime()
            //);
            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            //app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}