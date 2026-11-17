using System.Security.Claims;
using Data;
using DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Handlers;

public class AnimeHandler
{
    public static async Task<IResult> GetAll(AppDbContext db)
    {
        var anime = await db.Animes.Select(animeFromDb => new AnimeGetDTO
        {
            Name = animeFromDb.Name,
            Description = animeFromDb.Description,
            Genres = animeFromDb.Genres,
            ReleaseDate = animeFromDb.ReleaseDate
        }).ToListAsync();
        if(anime == null)
        {
            return Results.NotFound("Аніме не знайдено!");
        }
        return Results.Ok(anime);
    }

    public static async Task<IResult> Post(AnimePostDTO dto,IValidator<AnimePostDTO> validator, AppDbContext db)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        var animeExsists = await db.Animes.FirstOrDefaultAsync(x => x.Name == dto.Name);
        if (animeExsists != null)
        {
            return Results.BadRequest("Це аніме вже існує");
        }
        
        Anime anime = new Anime
        {
            Name = dto.Name,
            Description = dto.Description,
            Genres = dto.Genres,
            ReleaseDate = dto.ReleaseDate
        };
        db.Animes.Add(anime);
        await db.SaveChangesAsync();
        return Results.Created();
    }
    public static async Task<IResult> Put(int id,AnimePutDTO dto,IValidator<AnimePutDTO> validator,ClaimsPrincipal claimsUser,AppDbContext db)
    {   
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userRole = claimsUser.FindFirst(ClaimTypes.Role)?.Value;
        if(!claimsUser.IsInRole("Admin"))
        {
            return Results.BadRequest("Такі права доступні лише для адміна!");
        }

        var anime = await db.Animes.FindAsync(id);
        if(anime == null)
        {
            return Results.NotFound("Такого аніме нема!");
        }
        
        anime.Name = dto.Name ?? anime.Name;
        anime.Description = dto.Description ?? anime.Description;
        anime.Genres = dto.Genres ?? anime.Genres;
        anime.ReleaseDate = dto.ReleaseDate ?? anime.ReleaseDate;

        await db.SaveChangesAsync();
        return Results.Ok("Аніме змінено!");
    }

    public static async Task<IResult> Delete(int id,ClaimsPrincipal claimsUser,AppDbContext db)
    {
        var userRole = claimsUser.FindFirst(ClaimTypes.Role)?.Value;
        if(!claimsUser.IsInRole("Admin"))
        {
            return Results.BadRequest("Такі права доступні лише для адміна!");
        }

        var anime = await db.Animes.FindAsync(id);
        if(anime == null)
        {
            return Results.NotFound("Такого аніме нема!");
        }

        db.Animes.Remove(anime);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}