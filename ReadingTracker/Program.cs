using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;
using ReadingTracker.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ReadingTrackerDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("localDb")));
builder.Services.AddScoped<IBookDataAccess,  BookDataAccess>();

// HttpClient approach
builder.Services.AddHttpClient("ReadingTrackerApiClient", httpClient =>
{
    // set address in one place, instead of each usage of the client
    // address hard-coded, should be from configuration
    httpClient.BaseAddress = new Uri("https://localhost:7014");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Books/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();
