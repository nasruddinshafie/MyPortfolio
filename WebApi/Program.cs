using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Entity Framework
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Application and Infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
