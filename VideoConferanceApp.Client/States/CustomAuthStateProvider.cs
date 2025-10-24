using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace VideoConferanceApp.Client.States
{
    public class CustomAuthStateProvider(ILocalStorageService localStorageService, IConfiguration config) :
        AuthenticationStateProvider
    {
        ClaimsPrincipal User = new(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // get token key
            string key = config["Token:Key"]!;
            if (key == null) return await Task.FromResult(new AuthenticationState(User));

            // get token from loca storage
            var combinedToken = await localStorageService.GetItemAsStringAsync(key);
            if (combinedToken == null) return await Task.FromResult(new AuthenticationState(User));

            var jwtToken = combinedToken.Split('|')[0];
            User = SetClaim(jwtToken);
            return await Task.FromResult(new AuthenticationState(User));
        }

        public async Task SetUserAuthenticated(string combinedToken)
        {
            if (config["Token:Key"] == null) return;

            string key = config["Token:Key"]!;
            await localStorageService.SaveAsStringAsync(key, combinedToken);

            var jwtToken = combinedToken.Split('|')[0];
            User = SetClaim(jwtToken);
            NotifyAuthenticationStateChanged
                (Task.FromResult(new AuthenticationState(User)));
        }

        public async Task SetUserLoggedOut()
        {
            string key = config["Token:Key"]!;
            if (key == null) return;

            await localStorageService.DeleteItemAsync(key);
            User = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(User)));
        }

        private static ClaimsPrincipal SetClaim(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var claims = jwtToken.Claims;
                return new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));
            }
            catch
            {
                return new(new ClaimsIdentity());
            }
        }
    }
}