namespace VideoConferanceApp.Shared.Authentication.Responces;

public record RefreshTokenResponse(string? NewJwtToken = null!, string NewRefreshToken = null!) :
    ServiceResponse<string>;