using Handlers;
using Models;

namespace Endpoints;

public static class MapEndpoints
{
    public static void MapUserEndpoints(WebApplication app)
    {
        var userGroup = app.MapGroup("/users").RequireAuthorization();
        var authGroup = app.MapGroup("/users");
        userGroup.MapGet("/", UserHandler.GetAll).AllowAnonymous();
        userGroup.MapGet("/me",UserHandler.GetOne);
        authGroup.MapPost("/registration",UserHandler.Registration);
        authGroup.MapPost("/confirmEmail",UserHandler.ConfirmEmail);
        authGroup.MapPost("/login",UserHandler.Login);
        userGroup.MapPut("/me",UserHandler.Put);
        userGroup.MapDelete("/me",UserHandler.Delete);
    }

    public static void MapAnimeEndpoints(WebApplication app)
    {
        var animeGroup = app.MapGroup("/anime").RequireAuthorization();
        animeGroup.MapGet("/", AnimeHandler.GetAll).AllowAnonymous();
        animeGroup.MapPost("/",AnimeHandler.Post);
        animeGroup.MapPut("/{id}",AnimeHandler.Put);
        animeGroup.MapDelete("/{id}", AnimeHandler.Delete);
    }
}