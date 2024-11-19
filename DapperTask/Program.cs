
using Microsoft.Extensions.DependencyInjection;
using DapperTask.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Register repositories and inject the connection string for each



builder.Services.AddScoped<OrganizationRepository>(provider =>
    new OrganizationRepository(builder.Configuration.GetConnectionString("dbcs")));
builder.Services.AddScoped<DepartmentRepository>(provider =>
    new DepartmentRepository(builder.Configuration.GetConnectionString("dbcs")));
builder.Services.AddScoped<EmployeeRepository>(provider =>
    new EmployeeRepository(builder.Configuration.GetConnectionString("dbcs")));
builder.Services.AddScoped<PositionRepository>(provider =>
    new PositionRepository(builder.Configuration.GetConnectionString("dbcs")));

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
    pattern: "{controller=Home}/{action=Dashboard}/{id?}");

app.Run();
