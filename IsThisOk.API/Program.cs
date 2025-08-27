using IsThisOk.Application.Interfaces;
using IsThisOk.Application.Seeders;
using IsThisOk.Application.Services;
using IsThisOk.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Bind and register MongoDbSettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

var mongoDbSettings = builder.Configuration
    .GetSection(nameof(MongoDbSettings))
    .Get<MongoDbSettings>();

builder.Services.AddSingleton(mongoDbSettings);

// JWT Settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(nameof(JwtSettings)));

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(mongoDbSettings.ConnectionString));

// Register IMongoDatabase here
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<MongoDbSettings>();
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

//  Authentication Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationHelper, AuthorizationHelper>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Policy for regular users (anyone logged in)
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("User", "Admin")); // Both User and Admin can do user actions

    // Admins and SuperAdmin  
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin", "SuperAdmin"));

    // Policy for post owners (we'll check this manually)
    options.AddPolicy("PostOwnerPolicy", policy =>
        policy.RequireAuthenticatedUser());

    // Only SuperAdmin
    options.AddPolicy("SuperAdminPolicy", policy =>
        policy.RequireAuthenticatedUser()
              .RequireRole("SuperAdmin"));
});

builder.Services.AddSingleton<IRoleSeeder, RoleSeeder>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

//// Middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
    await seeder.SeedDefaultRolesAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
