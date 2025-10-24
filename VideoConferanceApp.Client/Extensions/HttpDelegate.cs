using System.Net;
using System.Net.Http.Headers;
using NetcodeHub.Packages.Extensions.LocalStorage;
using VideoConferanceApp.Client.Services;
using VideoConferanceApp.Shared.Authentication.Requests;
using VideoConferanceApp.Shared.Authentication.Responces;

namespace VideoConferanceApp.Client.Extensions;

public class HttpDelegate(IAuthService authService, IConfiguration config, ILocalStorageService localStorageService)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var result = await base.SendAsync(request, cancellationToken);

        if (result.StatusCode != HttpStatusCode.Unauthorized)
            return result;

        var bearerToken = result.RequestMessage!.Headers.Authorization!.Parameter!;
        if (bearerToken == null)
            return result;

        var key = config["Token:Key"]!;
        if (key == null)
            return result;

        var combinedToken = await localStorageService.GetItemAsStringAsync(key);
        if (combinedToken == null)
            return result;

        var refreshToken = combinedToken.Split('|')[1];
        if (refreshToken == null)
            return result;

        var response = await authService.RefreshToken(new RefreshTokenRequest(refreshToken));

        if (response!.IsSuccess)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", response.NewJwtToken);
            return await base.SendAsync(request, cancellationToken);
        }
        else
        {
            return result;
        }
    }
}