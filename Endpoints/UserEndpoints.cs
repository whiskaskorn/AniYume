using Handlers;

namespace Endpoints.Api;

public static class MapEndpoints
{
    public static void MapUserEndpoints(WebApplication app)
    {
        var baseGroup = app.MapGroup("/users").RequireAuthorization();
        var authGroup = app.MapGroup("/users");
        baseGroup.MapGet("/", UserHandler.GetAll).AllowAnonymous();
        baseGroup.MapGet("/me",UserHandler.GetOne);
        authGroup.MapPost("/registration",UserHandler.Registration);
        authGroup.MapPost("/confirmEmail",UserHandler.ConfirmEmail);
        authGroup.MapPost("/login",UserHandler.Login);
        baseGroup.MapPut("/",()=>{});
        baseGroup.MapDelete("/me",UserHandler.Delete);
    }
}