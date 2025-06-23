using Azure.Storage.Blobs;
using EVENT_EASE.Models;
using EVENT_EASE.Services; // Make sure this matches your folder structure
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Register the database context using the connection string
builder.Services.AddDbContext<Cldvdb1Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register the BlobStorageService
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("BlobStorage");
    var containerName = config.GetConnectionString("ContainerName"); // use config instead of hardcoded name
    return new BlobStorageService(connectionString, containerName);
});

var app = builder.Build();

// ✅ Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

