using Data;
using Endpoints.Api;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Handlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BcryptHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<UserContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:ConnectionPostGre"]));
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

MapEndpoints.MapUserEndpoints(app);
app.Run();