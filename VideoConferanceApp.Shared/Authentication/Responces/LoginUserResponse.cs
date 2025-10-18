using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoConferanceApp.Shared.Authentication.Responces
{
    public record LoginUserResponse(string JwtToken) : ServiceResponse<string>;
}
