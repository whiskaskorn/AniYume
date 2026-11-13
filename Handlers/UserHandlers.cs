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

namespace Handlers;

public class UserHandler
{
    public static int GenerateCode()
    {
        Random random = new Random();
        return random.Next(100000,999999);
    }

    public static async Task SendMessage(UserRegistrationDTO dto,int code, IConfiguration configuration)
    {
        var message = new MimeMessage();
        message.From.Add( new MailboxAddress("Whiskas", "whiskaskorn@gmail.com"));
        message.To.Add( new MailboxAddress(dto.Username,dto.Email!));
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
    public static async Task<IResult> GetAll(UserContext db)
    {   var users = await db.Users.ToListAsync();
        return Results.Ok(users);
    }
    public static async Task<IResult> Registration(UserRegistrationDTO dto, IValidator<UserRegistrationDTO> validator,UserContext db,BcryptHandler bcryptHandler, IConfiguration configuration)
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
        await SendMessage(dto,code, configuration);

        var User = new User{
            Username = dto.Username,
            Email = dto.Email,
            HashPassword = bcryptHandler.GenerateHashPass(dto.Password!),
            CodeForConfirmEmail = code
        };

        db.Users.Add(User);
        await db.SaveChangesAsync();
        return Results.Created();
    }

    public static async Task<IResult> ConfirmEmail(UserConfirmEmailDTO dto,IValidator<UserConfirmEmailDTO> validator, UserContext db)
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
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    public static async Task<IResult> Login(UserLoginDTO dto, IValidator<UserLoginDTO> validator,UserContext db)
    {
        var validationResult = validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if(user == null)
        {
            return Results.BadRequest("Користувача з такою поштою не знайдено!");
        }

        if (user.isExsistsEmail == false)
        {
            return Results.BadRequest("Для початку підтвердіть пошту!");
        }

        return Results.Ok();
    }
}