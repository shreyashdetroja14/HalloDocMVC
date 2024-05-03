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

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IPhysicianRepository, PhysicianRepository>();
builder.Services.AddScoped<INotesAndLogsRepository, NotesAndLogsRepository>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IEmailSMSLogRepository, EmailSMSLogRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();


builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRequestFormService, RequestFormService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProvidersService, ProvidersService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IRecordsService, RecordsService>();
builder.Services.AddScoped<IRoleAuthService, RoleAuthService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseStatusCodePagesWithReExecute("/Error/{0}");
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
