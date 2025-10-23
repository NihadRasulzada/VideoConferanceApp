using MediatR;
using Microsoft.AspNetCore.Identity;
using VideoConferanceApp.Server.Features.Authentication.CreateUserAccount.Command;
using VideoConferanceApp.Server.Models;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Server.Features.Authentication.CreateUserAccount.Handler
{
    public class CreateUserHandler(UserManager<AppUser> userManager) : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request.User);

            var user = new AppUser
            {
                Name = request.User.Name,
                UserName = request.User.Name,
                Email = request.User.Email
            };

            if (await userManager.FindByEmailAsync(user.Email) != null)
                return new() { IsSuccess = false, Message = "User allready exists" };

            var result =   await userManager.CreateAsync(user, request.User.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new() { IsSuccess = false, Message = $"User creation failed: {errors}" };
            }
            var dbUser = await userManager.FindByEmailAsync(user.Email);
            return new() { IsSuccess = true, Message = "New User Created" };
        }
    }
}