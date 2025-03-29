using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BonusSystem.Api.Features.Auth;

public static class AuthHandlers
{
    public static async Task<IResult> BuyerRegister(
        IAuthenticationService authService,
        BuyerRegistrationDto buyerRegistration)
    {
        var registration = new UserRegistrationDto
        {
            Username = buyerRegistration.UserName,
            Email = buyerRegistration.Email,
            Password = buyerRegistration.Password,
            Role = UserRole.Buyer
        };

        var result = await authService.SignUpAsync(registration);

        if (!result.Success)
        {
            return Results.BadRequest(new { message = result.ErrorMessage });
        }

        return Results.Ok(new
        {
            userId = result.UserId,
            token = result.Token,
            role = result.Role
        });
    }

    public static async Task<IResult> Register(
        IAuthenticationService authService,
        UserRegistrationDto registration)
    {
        var result = await authService.SignUpAsync(registration);

        if (!result.Success)
        {
            return Results.BadRequest(new { message = result.ErrorMessage });
        }

        return Results.Ok(new
        {
            userId = result.UserId,
            token = result.Token,
            role = result.Role
        });
    }

    public static async Task<IResult> Login(
        IAuthenticationService authService,
        UserLoginDto login)
    {
        var result = await authService.SignInAsync(login);

        if (!result.Success)
        {
            return Results.BadRequest(new { message = result.ErrorMessage });
        }

        return Results.Ok(new
        {
            userId = result.UserId,
            token = result.Token,
            role = result.Role
        });
    }
}