using VideoConferanceApp.Server.Features.Authentication.CreateUserAccount.Command;
using VideoConferanceApp.Server.Features.Authentication.LoginUser.Command;
using VideoConferanceApp.Shared.Authentication.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VideoConferanceApp.Server.Features.Authentication.GetNewToken.Command;
using VideoConferanceApp.Shared.Authentication.Responces;


namespace VideoConferanceApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CreateUserResponse>> CreateUser(CreateUserRequest user)
        {
            CreateUserResponse Response = await sender.Send(new CreateUserCommand(user));
            return Response.IsSuccess ? Ok(Response) : BadRequest(Response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> LoginUser(LoginUserRequest login)
        {
            LoginUserResponse Response = await sender.Send(new LoginUserCommand(login));
            return Response.IsSuccess ? Ok(Response) : BadRequest(Response);
        }

        [HttpPost("new-token")]
        public async Task<ActionResult<RefreshTokenResponse>>
            GetNewToken(RefreshTokenRequest token)
        {
            RefreshTokenResponse Response =
                await sender.Send(new CreateNewTokenCommand(token));
            return Response.IsSuccess ? Ok(Response) : BadRequest(Response);
        }
    }
}