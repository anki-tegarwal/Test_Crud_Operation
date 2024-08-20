using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Test_Crud_Operation;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.service;
using Test_Crud_Operation.service.Iservice;

var builder = WebApplication.CreateBuilder(args);

string cs = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<EmployeeDb>(option => option.UseSqlServer(cs));
builder.Services.AddScoped<IempService, EmpService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//jwt Authentication

var JwtSection = builder.Configuration.GetSection("JwtToken");
builder.Services.Configure<JwtToken>(JwtSection);
var appSetting = JwtSection.Get<JwtToken>();
var key = Encoding.ASCII.GetBytes(appSetting.secret);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,x =>
{
    x.LoginPath = "/Auth/Login"; // Path to login page
    x.LogoutPath = "/Auth/Logout"; // Path to logout page
    
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
