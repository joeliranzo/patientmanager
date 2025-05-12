using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Services;
using PatientManagement.Infrastructure.Data;
using PatientManagement.Infrastructure.Repositories;
using PatientManagement.Infrastructure.Security;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

//Adding global authorization -> example
//builder.Services.AddControllers(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();

//    options.Filters.Add(new AuthorizeFilter(policy));
//});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application & Domain services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();


// Infrastructure
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqlDbConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddSingleton<IEncryptionService>(_ =>
    new AesEncryptionService(builder.Configuration["Security:EncryptionKey"]!));

// JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

// Swagger + JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Patient Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
         {
             new OpenApiSecurityScheme {
                 Reference = new OpenApiReference {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             Array.Empty<string>()
         }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


await DatabaseInitializer.EnsureDatabaseCreatedAsync(builder.Configuration);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Urls.Add("https://localhost:5001");

app.Run();
