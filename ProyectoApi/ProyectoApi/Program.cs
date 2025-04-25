using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using ProyectoApi.Helpers;
using ProyectoApi.Interfaces;
using ProyectoApi.Repositories;
using ProyectoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Dapper
builder.Services.AddSingleton<DapperContext>();

// Repos & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

// Authentication: JWT & Cookie
builder.Services
  .AddAuthentication(options => {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options => {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
          ValidateIssuer = true,
          ValidIssuer = jwt.Issuer,
          ValidateAudience = true,
          ValidAudience = jwt.Audience,
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
      };
  });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa 'Bearer {token}'"
    });
    c.AddSecurityRequirement(new() {
    {
      new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        Reference = new() {
          Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
          Id   = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

