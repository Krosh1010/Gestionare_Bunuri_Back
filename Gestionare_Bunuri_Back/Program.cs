using Gestionare_Bunuri_Back.Middleware;
using Infrastructure.DataBase;
using Infrastructure.Dashboard;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction;
using Application.User;
using Application.Services;
using Application.Abstraction.CoverageStatus;
using Infrastructure.Abstraction;
using Infrastructure.Export;
using Infrastructure.Asset;
using Infrastructure.User;
using QuestPDF.Infrastructure;
using Infrastructure.Abstraction.CoverageStatus;
using Infrastructure.Asset.CoverageStatus;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options
     .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<Hash>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
builder.Services.AddScoped<ISpaceService, SpaceService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IWarrantyRepository, WarrantyRepository>();
builder.Services.AddScoped<IWarrantyService, WarrantyService>();
builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IWarrantyStatusRepository, WarrantyStatusRepository>();
builder.Services.AddScoped<IWarrantyStatusService, WarrantyStatusService>();
builder.Services.AddScoped<IInsuranceStatusRepository, InsuranceStatusRepository>();
builder.Services.AddScoped<IInsuranceStatusService, InsuranceStatusService>();
builder.Services.AddScoped<IExportRepository, ExportRepository>();
builder.Services.AddScoped<IExportService, ExportService>();



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
