namespace VideoConferanceApp.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VideoConferanceApp.Shared.Authentication.Requests;
    using VideoConferanceApp.Shared.Authentication.Responces;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<ActionResult<CreateUserResponse>> CreateUser(CreateUserRequest user)
        {
            CreateUserResponse response = await sender.Send(new CreateUserCommand(user));
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> LoginUser(LoginUserRequest login)
        {
            LoginUserResponse response = await sender.Send(new LoginUserCommand(login));
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}