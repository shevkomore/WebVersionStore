using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebVersionStore;
using WebVersionStore.Handlers;

var builder = WebApplication.CreateBuilder(args);

// The dependencies are defined in DependencyManager.cs 
builder.Services.AddDefaultDependencies();

//TODO Check how the parameter is *actually* used here
builder.Services.UseCustomHashPasswordBuilder().UseArgon2<WebVersionStore.Models.User>();

builder.Services.AddAuthorization();
//builder.Services.AddAuthentication("JwsAuthentication")
//    .AddScheme<AuthenticationSchemeOptions, JwsAuthenticationHandler>("JwsAuthentication", null);
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents()
    {
        OnTokenValidated = context =>
        {
            context.ValidateJwtToken();
            return Task.CompletedTask;
        }
    };
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("testkeyasdfxzcvzxcv12345678912356465445645564")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/");

app.Run();
