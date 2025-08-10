using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCenzontle.Responder.Persistencia;
using SCenzontle.Responder.Comun.Model;
using SCenzontle.Responder.Negocio.Servicios;
using SCenzontle.Responder.Comun.Configuracion;


var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos MDF
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configuración del servicio de registro
builder.Services.AddScoped<ServicioDeRegistro>();
// Agrega el servicio de autenticación
builder.Services.AddScoped<ServicioDeAutenticacion>();

// Configuración de roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("Broker", policy => policy.RequireRole("broker"));
    options.AddPolicy("Cliente", policy => policy.RequireRole("cliente"));
});

// Agrega servicios a la colección
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





CorsSettings corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsSettings.PolicyName,
        policy =>
        {
            policy.WithOrigins(corsSettings.AllowedOrigins)
                  .WithHeaders(corsSettings.AllowedHeaders)
                  .WithMethods(corsSettings.AllowedMethods);
        });
});

var app = builder.Build();

// Configura el middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsSettings.PolicyName);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Middleware para inicializar roles
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();

    async Task SeedRoles()
    {
        string[] roleNames = { "admin", "broker", "cliente" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    SeedRoles().Wait();
}



app.MapControllers();

app.Run();