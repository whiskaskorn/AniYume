using Handlers;

namespace Endpoints.Api;

public static class MapEndpoints
{
    public static void MapUserEndpoints(WebApplication app)
    {
        var baseGroup = app.MapGroup("/users");
        var authGroup = app.MapGroup("/users");
        baseGroup.MapGet("/", UserHandler.GetAll);
        baseGroup.MapGet("/me",()=>{});
        authGroup.MapPost("/registration",UserHandler.Registration);
        authGroup.MapPost("/confirmEmail",UserHandler.ConfirmEmail);
        authGroup.MapPost("/login",()=>{});
        baseGroup.MapPut("/",()=>{});
        baseGroup.MapDelete("/",()=>{});
    }
}