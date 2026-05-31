using ClinicManagementSystem.BL;       
using ClinicManagementSystem.BL.Logs;      
using ClinicManagementSystem.Data;
using ClinicManagementSystem.DL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<MedicineRepository>();
builder.Services.AddScoped<BillingRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ReceptionistRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<MedicalRepRepository>();
builder.Services.AddScoped<PrescriptionRepository>();
builder.Services.AddScoped<PatientListBL>();
builder.Services.AddScoped<AppointmentsBL>();
builder.Services.AddScoped<MedicalRepBL>();
builder.Services.AddScoped<MedicineStockBL>();
builder.Services.AddScoped<GenerateBillBL>();
builder.Services.AddScoped<RegisterPatientBL>();
builder.Services.AddScoped<RegisterDoctorBL>();
builder.Services.AddScoped<RegisterStaffBL>();
builder.Services.AddScoped<ReportsBL>();
builder.Services.AddScoped<SettingsBL>();
builder.Services.AddScoped<ClinicOwnerDashboardBL>();
builder.Services.AddScoped<ReceptionistDashboardBL>();
builder.Services.AddScoped<DoctorDashboardBL>();
builder.Services.AddScoped<FollowUpBL>();
builder.Services.AddScoped<PrescriptionBL>();
builder.Services.AddScoped<ViewAppointmentsBL>();
builder.Services.AddScoped<PaymentsBL>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<SettingsRepository>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Public/Index");
    return Task.CompletedTask;
});

app.Run();