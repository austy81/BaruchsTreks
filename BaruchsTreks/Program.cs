using BaruchsTreks.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Set the challenge scheme to OpenID Connect
})
//.AddCookie()
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd")); // Configure Microsoft Identity Web App
//.AddGoogle(options =>
//{
//    options.ClientId = Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
//}); // Configure Google authentication

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services
    .AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var config = builder.Configuration;


        var databaseName = config.GetValue<string>("CosmosDatabaseName");
        var cosmosDbConnectionString = config.GetValue<string>("COSMOSDB_CONNECTION_STRING");

        _ = options.UseCosmos(cosmosDbConnectionString, databaseName);
    });
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    context.Add(new Trip()
    {
        Title = "Demo",
        LengthHours = 8
    });

    //string[] roles = new string[] { "owner", "visitor" };

    //foreach (string role in roles)
    //{
    //    context.Add(new IdentityRole(role));
    //}

    //var user = new IdentityUser
    //{
    //    Email = "xxxx@example.com",
    //    NormalizedEmail = "XXXX@EXAMPLE.COM",
    //    UserName = "Owner",
    //    NormalizedUserName = "OWNER",
    //    PhoneNumber = "+111111111111",
    //    EmailConfirmed = true,
    //    PhoneNumberConfirmed = true,
    //    SecurityStamp = Guid.NewGuid().ToString("D"),
    //};
    //context.Add(user);

    await context.SaveChangesAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
