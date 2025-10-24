using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VideoConferanceApp.Server.Features.Authentication.LoginUser.Command;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Models;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.LoginUser.Handler
{
    public class LoginUserHandler(UserManager<AppUser> userManager, AppDbContext context,
       ITokenGenerator tokenGenerator, IHttpContextAccessor httpContext) : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        public async Task<LoginUserResponse>
            Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // check id model is NULL
            ArgumentNullException.ThrowIfNull(request.Login);

            var user = await userManager.FindByEmailAsync(request.Login.Email);
            if (user == null)
                return new(null!)
                { IsSuccess = false, Message = "Invalid Credentials provided" };

            if (!await userManager.CheckPasswordAsync(user, request.Login.Password))
                return new(null!)
                { IsSuccess = false, Message = "Invalid Credentials provided" };

            var userClaims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            string token = tokenGenerator.GenerateToken(userClaims);
            string newRefreshToken = tokenGenerator.GenerateRefreshToken();

            // check if NULL
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newRefreshToken))
                return new(null!, null!) 
                { IsSuccess = false, Message = "Something went wrong" };

            // get ip address
            var ipAddress = httpContext.HttpContext!.Connection.RemoteIpAddress?.ToString();

            // Update token info when user re-login
            var tokenInfo = await context.RefreshTokens
                .FirstOrDefaultAsync
                (_ => _.UserId.Equals(user.Id), cancellationToken: cancellationToken);
            if (tokenInfo != null)
            {
                tokenInfo.Token = newRefreshToken;
                tokenInfo.IpAddress = ipAddress;
            }
            else
            {
                // save the refresh token with user details
                context.RefreshTokens.Add(new RefreshToken
                {
                    CreatedAt = DateTime.UtcNow,
                    Token = newRefreshToken,
                    IpAddress = ipAddress ?? "NULL",
                    UserId = user.Id
                });
            }

            // save updated or newly added
            await context.SaveChangesAsync(cancellationToken);

            // return token and refresh token

            return new(token, newRefreshToken)
            {
                IsSuccess = true,
                Message = "Login made successfully",
            };
        }
    }
}