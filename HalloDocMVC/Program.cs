using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Repository;
using HalloDocRepository.Repository.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HalloDocContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("HalloDocDbCS")));

builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAspNetUserRepository, AspNetUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
