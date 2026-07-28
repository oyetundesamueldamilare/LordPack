using LordPack.Api.Data;
using LordPack.Api.Interfaces;
using LordPack.Api.Repositories;
using LordPack.Api.Services;
using LordPack.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection String & Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity Configuration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>();

// 3. JWT Authentication Setup
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretLordPackKey2026!KeyMustBe32BytesMinimum";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LordPack.Api",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LordPack.Mobile",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// 4. Dependency Injection
builder.Services.AddScoped<IAudioBookRepository, AudioBookRepository>();
builder.Services.AddScoped<IDevotionalRepository, DevotionalRepository>();

builder.Services.AddScoped<IAudioBookService, AudioBookService>();
builder.Services.AddScoped<IDevotionalService, DevotionalService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

// 5. Swagger Setup (Replaces AddOpenApi)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Database Initialization & Seeding
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(dbContext);
}

// 7. Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();