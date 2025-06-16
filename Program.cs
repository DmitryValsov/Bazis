using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bazis.Data;
using Telegram.Bot;
using Bazis.Hubs;
using Bazis.Services;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();


// Telegram.Bot client
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    new TelegramBotClient(builder.Configuration["Telegram:BotToken"])
);

// BackgroundService для «обратки» из Telegram
builder.Services.AddHostedService<TelegramRelayService>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using(var scope = app.Services.CreateScope())
{
    await IdentityDataSeeder.SeedAsync(scope.ServiceProvider);
}



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

/*
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapDefaultControllerRoute();
});
*/
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
 name: "areas",
 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
