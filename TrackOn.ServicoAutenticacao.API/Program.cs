using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrackOn.ServicoAutenticacao.API.Infra.Repository;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Services;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using TrackOn.ServicoAutenticacao.API.Config;
using TrackOn.ServicoAutenticacao.API.Settings;


var builder = WebApplication.CreateBuilder(args);

var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()
    ?? throw new InvalidOperationException("As configurações de JWT não foram encontradas.");
jwtConfig.EnsureValid();

var corsConfig = builder.Configuration.GetSection("Cors").Get<CorsConfig>()
    ?? throw new InvalidOperationException("As configurações de CORS não foram encontradas.");
corsConfig.EnsureValid();

builder.Services.AddOptions<JwtConfig>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(settings =>
    {
        try
        {
            settings.EnsureValid();
            return true;
        }
        catch (ValidationException)
        {
            return false;
        }
    }, "As configurações de JWT são inválidas.")
    .ValidateOnStart();

builder.Services.AddOptions<CorsConfig>()
    .Bind(builder.Configuration.GetSection("Cors"))
    .Validate(settings =>
    {
        try
        {
            settings.EnsureValid();
            return true;
        }
        catch (ValidationException)
        {
            return false;
        }
    }, "As configurações de CORS são inválidas.")
    .ValidateOnStart();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsConfig.PolicyName, policy =>
        policy.WithOrigins(corsConfig.AllowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

var key = Encoding.UTF8.GetBytes(jwtConfig.Key); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// --- USAR CORS ---
app.UseCors(corsConfig.PolicyName);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
