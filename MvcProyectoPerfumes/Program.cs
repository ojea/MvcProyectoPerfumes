using Microsoft.EntityFrameworkCore;
using MvcCoreCryptography.Repositories;
using MvcProyectoPerfumes.Data;
using MvcProyectoPerfumes.Helpers;
using MvcProyectoPerfumes.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<RepositoryPerfumes>();
builder.Services.AddTransient<RepositoryUsuarios>();
builder.Services.AddTransient<HelperMails>();
builder.Services.AddTransient<HelperPathProvider>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddSingleton<IHttpContextAccessor
    , HttpContextAccessor>();

string connectionString = builder.Configuration.GetConnectionString("SqlPerfumes");

builder.Services.AddDbContext<PerfumesContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddDbContext<UsuariosContext>
    (options => options.UseSqlServer(connectionString));
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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Perfumes}/{action=PaginarGrupoPerfumes}/{id?}");

app.Run();
