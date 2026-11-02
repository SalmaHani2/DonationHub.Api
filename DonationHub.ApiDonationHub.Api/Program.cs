
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed Admin
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await seeder.SeedAsync();
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
