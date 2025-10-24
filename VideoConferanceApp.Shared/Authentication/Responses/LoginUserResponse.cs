namespace VideoConferanceApp.Shared.Authentication.Responces
{
    public record LoginUserResponse(string JwtToken, string RefreshToken = null!) : ServiceResponse<string>;
}