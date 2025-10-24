using System.Net.Http.Json;
using NetcodeHub.Packages.Extensions.LocalStorage;
using VideoConferanceApp.Client.Extensions;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Client.Services
{
    public interface IAuthService
    {
        Task<LoginUserResponse?> Login(LoginUserRequest user);
        Task<CreateUserResponse?> CreateUserAccount(CreateUserRequest user);
        Task<RefreshTokenResponse?> RefreshToken(RefreshTokenRequest refreshTokenRequest);
    }

    public class AuthService(
        IHttpExtension httpExtension,
        ILocalStorageService localStorageService,
        IConfiguration config) : IAuthService
    {
        public async Task<CreateUserResponse?> CreateUserAccount(CreateUserRequest user)
        {
            try
            {
                var result = await httpExtension
                    .GetPublicClient().PostAsJsonAsync("auth/create", user);
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
                var result = await httpExtension.GetPublicClient()
                    .PostAsJsonAsync("auth/login", user);
                return await result.Content.ReadFromJsonAsync<LoginUserResponse>();
            }
            catch
            {
                return new LoginUserResponse(null!)
                    { IsSuccess = false, Message = "Error connecting to server" };
            }
        }

        public async Task<RefreshTokenResponse?> RefreshToken
            (RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var result = await httpExtension.GetPublicClient()
                    .PostAsJsonAsync("auth/new-token", refreshTokenRequest);
                var response = await result.Content
                    .ReadFromJsonAsync<RefreshTokenResponse>();
                if (response!.IsSuccess)
                {
                    // save to localStorage
                    var key = config["Token:Key"]!;
                    var combinedToken = $"{response!.NewJwtToken}|{response.NewRefreshToken}";
                    await localStorageService.SaveAsStringAsync(key, combinedToken);
                }

                return response;
            }
            catch
            {
                return new RefreshTokenResponse(null, null!)
                    { IsSuccess = false, Message = "Error connecting to server" };
            }
        }
    }
}