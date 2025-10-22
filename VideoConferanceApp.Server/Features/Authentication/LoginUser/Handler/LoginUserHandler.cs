using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using VideoConferanceApp.Server.Features.Authentication.LoginUser.Command;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Models;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.LoginUser.Handler
{
    public class LoginUserHandler(
        UserManager<AppUser> userManager,
        AppDbContext context,
        ITokenGenerator tokenGenerator) : IRequestHandler<LoginUserCommand, LoginUserResponse>
    {
        public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.Login);

            var user = await userManager.FindByEmailAsync(request.Login.Email);

            if (user == null)
                return new(null!) { IsSuccess = false, Message = "Invalid Credentials provided." };

            if (!await userManager.CheckPasswordAsync(user, request.Login.Password))
                return new(null!) { IsSuccess = false, Message = "Invalid Credentials provided." };

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email!),
            };

            string token = tokenGenerator.GenerateToken(userClaims);
            return new(token) { IsSuccess = true, Message = "Login made successfully" };
        }
    }
}