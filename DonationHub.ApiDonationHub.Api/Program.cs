using DonationHub.Application.Interfaces;
using DonationHub.Application.Options;
using DonationHub.Domain.ValueObject;
using DonationHub.Domain.ValueObject;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Services;
using DonationHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        // Password Policy
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;

        // Unique Email
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<JwtTokenService>();
builder.Services
    .AddOptions<SeedAdminOptions>()
    .Bind(builder.Configuration.GetSection(SeedAdminOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<DonationOptions>()
    .Bind(builder.Configuration.GetSection("Donation"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


////////////////////////////////////////////////////////////////////////////////////
builder.Services.AddSingleton<SoftDeleteInterceptor>();
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));

    options.AddInterceptors(
        serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
});
//////////////////////////////////////////////////
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//////////////////////////////////////////////////
builder.Services.AddScoped<AdminSeeder>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
