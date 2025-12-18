using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CollegeEventsApi.Data;
using CollegeEventsApi.Profiles;
using CollegeEventsApi.Repositories;
using CollegeEventsApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? "Data Source=collegeevents.db";

if (builder.Environment.IsProduction())
{
    conn = "Data Source=/data/collegeevents.db";
}

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(conn));


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();


builder.Services.AddCors(options =>
{
    options.AddPolicy("blazor", policy =>
        policy.WithOrigins(
                "http://localhost:5191",
                "https://localhost:7158",
                "https://collegeeventsblazor.fly.dev"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


var jwtSection = builder.Configuration.GetSection("Jwt");
var keyStr = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(keyStr))
    throw new Exception("Jwt:Key missing from configuration");

var key = Encoding.UTF8.GetBytes(keyStr);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();



app.UseRouting();

app.UseCors("blazor"); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
