using HalloDocEntities.Data;
using HalloDocRepository.Implementation;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HalloDocContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("HalloDocDbCS")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IPhysicianRepository, PhysicianRepository>();
builder.Services.AddScoped<INotesAndLogsRepository, NotesAndLogsRepository>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();


builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRequestFormService, RequestFormService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();


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

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=AdminDashboard}/{action=Index}/{id?}");

app.Run();
