using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Test_Crud_Operation;
using Test_Crud_Operation.Authorization;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.service;
using Test_Crud_Operation.service.Iservice;

var builder = WebApplication.CreateBuilder(args);

string cs = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<EmployeeDb>(option => option.UseSqlServer(cs));
builder.Services.AddScoped<IempService, EmpService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IjwtUtils, JwtUtils>();

//jwt Authentication

var JwtSection = builder.Configuration.GetSection("JwtToken");
builder.Services.Configure<JwtToken>(JwtSection);
var appSetting = JwtSection.Get<JwtToken>();
var key = Encoding.ASCII.GetBytes(appSetting.secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login"; // Customize your login path
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
app.UseMiddleware<JwtMiddleware>();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
