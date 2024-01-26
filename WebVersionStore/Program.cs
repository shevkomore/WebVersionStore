using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebVersionStore;
using WebVersionStore.Handlers;
using WebVersionStore.Models;
using WebVersionStore.Models.Database;
using WebVersionStore.Models.Local;

var builder = WebApplication.CreateBuilder(args);

var section = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(section);

// The dependencies are defined in DependencyManager.cs 
builder.Services.AddDefaultDependencies(builder.Configuration);

//The Argon2 algotrithm here doesn't use the given type at all!
//It can technically be any type
//The User type has been chosen simply because it makes (conceptual) sense.
builder.Services.UseCustomHashPasswordBuilder().UseArgon2<User>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).SetupJwtBearer(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/");

app.Run();
