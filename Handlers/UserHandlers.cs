using MailKit;
using Data;
using DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Models;
using Validation;
using System.Security.Claims;

namespace Handlers;

public class UserHandler
{
    public static int GenerateCode()
    {
        Random random = new Random();
        return random.Next(100000,999999);
    }

    public static async Task SendMessage(string Username,string Email,int code, IConfiguration configuration)
    {
        var message = new MimeMessage();
        message.From.Add( new MailboxAddress("Whiskas", "whiskaskorn@gmail.com"));
        message.To.Add( new MailboxAddress(Username,Email));
        message.Subject = $"Code: {code}";
        message.Body = new TextPart("plain")
        {
            Text = @"Для підтвердження емайлу скористайтесь кодом!"
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com",587,MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("whiskaskorn@gmail.com", configuration.GetValue<string>("ApiKeys:PasswordForSendEmail")
                ?? throw new InvalidOperationException("Не заданий пароль для відправки емайлу!"));
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

    public static async Task<IResult> GetAll(AppDbContext db)
    {   var user = await db.Users.Select(userFromDb => new UserGetDTO
        {
            Id = userFromDb.Id,
            Username = userFromDb.Username,
            Email = userFromDb.Email
        }).ToListAsync();
        return Results.Ok(user);
    }

    public static async Task<IResult> GetOne(ClaimsPrincipal claimsUser,AppDbContext db)
    {
        var userIdString = claimsUser.FindFirstValue("Id");
        if(userIdString == null)
        {
            return Results.BadRequest("В клаймах айді не знайдений!");
        }
        int userIdInt = Convert.ToInt32(userIdString);
        var user = await db.Users.Where(x => x.Id == userIdInt).Select(userFromDb => new UserGetDTO
        {
            Id = userFromDb.Id,
            Username = userFromDb.Username,
            Email = userFromDb.Email
        }).ToListAsync();

        return Results.Ok(user);
    }
    public static async Task<IResult> Registration(UserRegistrationDTO dto, IValidator<UserRegistrationDTO> validator,AppDbContext db,BcryptHandler bcryptHandler, IConfiguration configuration)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var isExistsUser = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);
        if (isExistsUser != null)
        {
            return Results.Conflict("Користувач з таким іменем чи поштою вже існує!");
        }

        int code = GenerateCode();

        var User = new User{
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = bcryptHandler.GenerateHashPass(dto.Password!),
            CodeForConfirmEmail = code,
            Roles = ["User"]
        };

        db.Users.Add(User);
        await db.SaveChangesAsync();

        await SendMessage(dto.Username!,dto.Email!,code, configuration);
        return Results.Created();
    }
    public static async Task<IResult> ConfirmEmail(UserConfirmEmailDTO dto,IValidator<UserConfirmEmailDTO> validator, AppDbContext db)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email && x.CodeForConfirmEmail == dto.Code);
        if(user == null)
        {
            return Results.BadRequest("Неправильна пошта чи код!");
        }

        user.isExsistsEmail = true;
        user.CodeForConfirmEmail = null;
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    public static async Task<IResult> Login(UserLoginDTO dto, IValidator<UserLoginDTO> validator,AppDbContext db,BcryptHandler bcryptHandler, JwtHandler jwtHandler)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if(user == null)
        {
            return Results.BadRequest("Неправильна пошта або пароль!");
        }

        if (user.isExsistsEmail == false)
        {
            return Results.BadRequest("Для початку підтвердіть пошту!");
        }

        if(!bcryptHandler.VerifyHashPass(dto.Password!, user.HashPassword!))
        {
            return Results.BadRequest("Неправильна пошта або пароль!");
        }

        var jwtToken = jwtHandler.GenerateToken(user);
        
        return Results.Ok(jwtToken);
    }

    public static async Task<IResult> Put(UserPutDTO dto,IValidator<UserPutDTO> validator,ClaimsPrincipal claimsUser,BcryptHandler bcryptHandler, AppDbContext db,IConfiguration configuration)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var userIdString = claimsUser.FindFirstValue("Id");
        if(userIdString == null)
        {
            return Results.BadRequest("В клаймах айді не знайдений!");
        }
        int userIdInt = int.Parse(userIdString);
        var user = await db.Users.FindAsync(userIdInt);
        if(user == null)
        {
            return Results.BadRequest("Користувач не знайден!");
        }
        
        var isEsxistsUser = await db.Users.FirstOrDefaultAsync(x => x.Id != userIdInt && (x.Username == dto.Username || x.Email == dto.Email));
        if (isEsxistsUser != null)
        {
            return Results.BadRequest("Користувач з таким іменем чи поштою вже існує!");
        }
        if (!bcryptHandler.VerifyHashPass(dto.CurrentPassword!, user.HashPassword!))
        {
            return Results.Conflict("Неправильний пароль!");
        }

        if(dto.Username != null)
        {
            user.Username = dto.Username;
        }

        if(dto.NewPassword != null)
        {
            user.HashPassword = bcryptHandler.GenerateHashPass(dto.NewPassword);
        }
        bool emailChanged = dto.Email != null && dto.Email != user.Email;

        if (emailChanged)
        {   
            user.Email = dto.Email;
            user.isExsistsEmail = false;
            user.CodeForConfirmEmail = GenerateCode();
            await SendMessage(user.Username!,user.Email!,user.CodeForConfirmEmail.Value,configuration);
            await db.SaveChangesAsync();
            return Results.Ok("Користувач змінений. Підтвердіть пошту для підтвердження емайлу!");
        }
        await db.SaveChangesAsync();
        return Results.Ok("Користувач змінений!");
    }

    public static async Task<IResult> Delete(ClaimsPrincipal claimsUser,AppDbContext db)
    {
        var userIdString = claimsUser.FindFirstValue("Id");
        if(userIdString == null)
        {
            return Results.BadRequest("В клаймах айді не знайдений!");
        }
        int userIdInt = Convert.ToInt32(userIdString);
        var user = await db.Users.FindAsync(userIdInt);
        if(user == null)
        {
            return Results.NotFound("Такого користувача не існує");
        }
        
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}