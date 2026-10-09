using System.Text;
using DigitalMenu.Entities;
using DigitalMenu.Middleware;
using DigitalMenu.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. REGISTRACIJA OSNOVNIH SERVISA & BAZA
// ==========================================

// Dinamička CORS konfiguracija u zavisnosti od okruženja (Dev / Prod)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // U Razvoju: Dozvoli sve origins (localhost, IP adrese, mobitele)
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // U Produkciji (Railway): Dozvoli moje domene + custom domene klijenata
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;

                var uri = new Uri(origin);

                if (uri.Host.EndsWith("vercel.app") || 
                    uri.Host.EndsWith("digitalmenu.com") || 
                    uri.Host == "localhost")
                {
                    return true;
                }

                return true;
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
    });
});

// Sigurno čitanje konekcijskog stringa za PostgreSQL bazu
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                         ?? Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? Environment.GetEnvironmentVariable("POSTGRES_URL")
                         ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Fallback: Ako varijabla još nije učitana kao URL, sastavi je iz Railway PostgreSQL varijabli
if (string.IsNullOrEmpty(connectionString))
{
    var pgHost = Environment.GetEnvironmentVariable("PGHOST");
    var pgPort = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var pgUser = Environment.GetEnvironmentVariable("PGUSER");
    var pgPass = Environment.GetEnvironmentVariable("PGPASSWORD");
    var pgDb   = Environment.GetEnvironmentVariable("PGDATABASE");

    if (!string.IsNullOrEmpty(pgHost) && !string.IsNullOrEmpty(pgUser))
    {
        connectionString = $"Host={pgHost};Port={pgPort};Username={pgUser};Password={pgPass};Database={pgDb};";
    }
}

// Konverzija URL formata (postgres:// ili postgresql://) u standardni ADO.NET Npgsql format
if (!string.IsNullOrEmpty(connectionString) && (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://")))
{
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');
    var user = userInfo.Length > 0 ? userInfo[0] : "";
    var password = userInfo.Length > 1 ? userInfo[1] : "";
    var port = databaseUri.Port > 0 ? databaseUri.Port : 5432;
    var database = databaseUri.LocalPath.TrimStart('/');

    connectionString = $"Host={databaseUri.Host};Port={port};Username={user};Password={password};Database={database};Ssl Mode=Prefer;";
}

// Ako konekcijski string i dalje nije pronađen, postavi fallback da se aplikacija ne ruši pri startu
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("UPOZORENJE: Connection string nije pronađen u varijablama okruženja!");
    connectionString = "Host=localhost;Database=dummy;Username=dummy;Password=dummy;";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registracija aplikacijskih servisa
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

// Zaštita i tenant izolacija
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

// ==========================================
// 2. KONFIGURACIJA JWT AUTHENTICATION-A
// ==========================================
var jwtSecret = builder.Configuration["Jwt:Secret"] 
             ?? Environment.GetEnvironmentVariable("Jwt__Secret")
             ?? "SuperTajniDefaultniJWTKljucKojiImaViseOd32Znakova12345!";

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidIssuer = jwtIssuer,
        
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidAudience = jwtAudience,
        
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ==========================================
// 3. KONTROLERI I SWAGGER
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Digital Menu API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Unesite JWT token u formatu: Bearer {vaš_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==========================================
// 4. AUTOMATSKO POKRETANJE MIGRACIJA BAZA
// ==========================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Automatski izvršava EF Core migracije i kreira tabele pri startu na Railway-u
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Došlo je do greške pri automatskom pokretanju migracija baze podataka.");
    }
}

// ==========================================
// 5. HTTP PIPELINE (MIDDLEWARE REDOSLIJED)
// ==========================================

app.UseMiddleware<ExceptionMiddleware>();

// Swagger je omogućen u svim okruženjima (za testiranje na Railway-u)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Menu API v1");
    c.RoutePrefix = "swagger"; // Dostupno na /swagger
});

// Preusmjeravanje sa bazne adrese (/) na /swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Omogućava posluživanje statičkih fajlova ako ih bude u wwwroot
app.UseStaticFiles();

// Aktivacija CORS-a (mora biti prije Authentication i Authorization)
app.UseCors("AllowFrontend");

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();