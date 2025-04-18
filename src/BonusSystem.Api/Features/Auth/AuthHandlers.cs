using BonusSystem.Api.Helpers;
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
        try
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
                return RequestHelper.CreateErrorResponse(result.ErrorMessage);
            }

            return RequestHelper.CreateSuccessResponse(new
            {
                userId = result.UserId,
                token = result.Token,
                role = result.Role
            });
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error during buyer registration");
        }
    }

    public static async Task<IResult> Register(
        IAuthenticationService authService,
        UserRegistrationDto registration)
    {
        try
        {
            var result = await authService.SignUpAsync(registration);

            if (!result.Success)
            {
                return RequestHelper.CreateErrorResponse(result.ErrorMessage);
            }

            return RequestHelper.CreateSuccessResponse(new
            {
                userId = result.UserId,
                token = result.Token,
                role = result.Role
            });
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error during user registration");
        }
    }

    public static async Task<IResult> Login(
        IAuthenticationService authService,
        UserLoginDto login)
    {
        try
        {
            var result = await authService.SignInAsync(login);

            if (!result.Success)
            {
                return RequestHelper.CreateErrorResponse(result.ErrorMessage);
            }

            return RequestHelper.CreateSuccessResponse(new
            {
                userId = result.UserId,
                token = result.Token,
                role = result.Role
            });
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error during user login");
        }
    }
}