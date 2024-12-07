using Carter;
using Microsoft.EntityFrameworkCore;
using vertical_slice.Database;
using vertical_slice.Features.Articles;
using FluentValidation;

namespace vertical_slice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

            var assembly = typeof(Program).Assembly;

            builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

            builder.Services.AddCarter();

            builder.Services.AddValidatorsFromAssembly(assembly);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.MapCarter();

            app.Run();
        }
    }
}
