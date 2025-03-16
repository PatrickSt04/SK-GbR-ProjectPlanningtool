using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;


var builder = WebApplication.CreateBuilder(args);




builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()  // F�ge die Rollenunterst�tzung hinzu
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
       .Build();
});

// Ausnahme für Identity-Routen hinzufügen
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//include DbInitializer
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        context.Database.EnsureCreated();


        await DbInitializer.Initialize(context, userManager, roleManager);
    }

}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapDefaultControllerRoute();
});

app.Run();
