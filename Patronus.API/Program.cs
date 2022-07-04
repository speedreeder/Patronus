using Patronus.DAL;
using Microsoft.EntityFrameworkCore;
using Patronus.API.Services;
using FluentValidation.AspNetCore;
using Patronus.Api.Models.Validators;

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
            builder.Services.AddControllers().AddFluentValidation(s =>
            {
                s.RegisterValidatorsFromAssemblyContaining<ContactDtoValidator>();
            });

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<PatronusContext>();
                var created = context.Database.EnsureCreated();
                if (created)
                {
                    DbInitializer.Initialize(context);
                }
            }

            app.Run();
        }
    }
}