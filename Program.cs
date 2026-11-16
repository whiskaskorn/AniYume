using Data;
using Endpoints;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Handlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<JwtHandler>();
builder.Services.AddScoped<BcryptHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var jwtHandler = new JwtHandler(builder.Configuration);
jwtHandler.ConfigureToken(builder.Services);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:ConnectionPostGre"]));
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

MapEndpoints.MapUserEndpoints(app);
MapEndpoints.MapAnimeEndpoints(app);
app.Run();