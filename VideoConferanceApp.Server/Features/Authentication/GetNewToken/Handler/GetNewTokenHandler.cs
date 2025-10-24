using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Authentication.GetNewToken.Command;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Models;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.GetNewToken.Handler;

public class GetNewTokenHandler(
    UserManager<AppUser> userManager,
    AppDbContext context,
    ITokenGenerator tokenGenerator,
    IHttpContextAccessor httpContext) :
    IRequestHandler<CreateNewTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse>
        Handle(CreateNewTokenCommand request, CancellationToken cancellationToken)
    {
        // check if model is NULL
        ArgumentNullException.ThrowIfNull(request.RefreshToken);

        // find refresh token from table
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(_ => _.Token
                    .Equals(request.RefreshToken.RefreshToken),
                cancellationToken: cancellationToken);

        if (token == null)
            return new() { IsSuccess = false, Message = "Invalid token provided" };

        //find user details by Id
        var user = await userManager.FindByIdAsync(token.UserId);
        if (user == null)
            return new() { IsSuccess = false, Message = "User not found" };

        var ipAddress = httpContext.HttpContext!.Connection.RemoteIpAddress?.ToString();

        // remove the refresh token when request ip address changes
        if (!token.IpAddress!.Equals(ipAddress))
        {
            context.RefreshTokens.Remove(token);
            await context.SaveChangesAsync(cancellationToken);
            return new()
            {
                IsSuccess = false,
                Message = "Token expired, kindly re-login"
            };
        }

        // get user  claims
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        // create neew token and refresh token
        string newToken = tokenGenerator.GenerateToken(userClaims);
        string newRefreshToken = tokenGenerator.GenerateRefreshToken();

        // check if NULL
        if (string.IsNullOrEmpty(newToken) || string.IsNullOrEmpty(newRefreshToken))
            return new() { IsSuccess = false, Message = "Something went wrong!" };

        // update the token Info since we have already added when user logs in
        token.Token = newRefreshToken;
        await context.SaveChangesAsync(cancellationToken);

        // return new token and refresh token
        return new()
        {
            IsSuccess = true,
            Message = "New token successfully generated",
            NewJwtToken = newToken,
            NewRefreshToken = newRefreshToken
        };
    }
}