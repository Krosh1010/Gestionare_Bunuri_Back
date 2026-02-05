using Gestionare_Bunuri_Back.Middleware;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction;
using Application.User;
using Application.Services;
using Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options
     .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Hash>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISpaceService, SpaceService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IWarrantyService, WarrantyService>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();


builder.Services.AddCors(option =>
{
    option.AddPolicy("Policy", builder =>
    {
        builder
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Policy");

app.UseAuthentication();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
