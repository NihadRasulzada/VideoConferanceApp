namespace VideoConferanceApp.Shared.Authentication.Responces
{
    public record LoginUserResponse(string JwtToken) : ServiceResponse<string>;
}
