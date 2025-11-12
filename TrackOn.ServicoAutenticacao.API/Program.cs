using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrackOn.ServicoAutenticacao.API.Infra.Repository;
using TrackOn.ServicoAutenticacao.API.Infra.Repository.Interfaces;
using TrackOn.ServicoAutenticacao.API.Services;
using TrackOn.ServicoAutenticacao.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- HABILITAR CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontendLocal", policy =>
        policy
            .WithOrigins("http://localhost:3000") // frontend React
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // use se precisar enviar cookies/autenticação
    );
});



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IServicoAutenticacao, ServicoAutenticacao>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
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
app.UseCors("PermitirFrontendLocal");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
