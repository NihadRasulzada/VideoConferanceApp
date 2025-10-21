using System.Net.Http.Json;
using VideoConferanceApp.Client.Extensions;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Client.Services
{
    public interface IAuthService
    {
        Task<LoginUserResponse?> Login(LoginUserRequest user);
        Task<CreateUserResponse?> CreateUserAccount(CreateUserRequest user);
    }

    public class AuthService(IHttpExtension httpExtension) : IAuthService
    {
        public async Task<CreateUserResponse?> CreateUserAccount(CreateUserRequest user)
        {
            try
            {
                var result = await httpExtension.GetPublicClient().PostAsJsonAsync("auth/create", user);
                return await result.Content.ReadFromJsonAsync<CreateUserResponse>();
            }
            catch
            {
                return new CreateUserResponse { IsSuccess = false, Message = "Error connecting to server" };
            }
        }

        public async Task<LoginUserResponse?> Login(LoginUserRequest user)
        {
            try
            {
                var result = await httpExtension.GetPublicClient().PostAsJsonAsync("auth/login", user);
                return await result.Content.ReadFromJsonAsync<LoginUserResponse>();
            }
            catch
            {
                return new LoginUserResponse(null!) { IsSuccess = false, Message = "Error connecting to server" };
            }
        }
    }
}
