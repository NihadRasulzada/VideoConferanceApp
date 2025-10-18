using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoConferanceApp.Shared
{
    public abstract record ServiceResponse<T>(bool IsSuccess = false, string? Message = null, T? Data = default);
}
