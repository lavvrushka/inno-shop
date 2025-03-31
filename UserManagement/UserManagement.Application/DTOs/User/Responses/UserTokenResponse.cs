using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs.User.Responses
{
    public record UserTokenRespones(
         string AccessToken,
         string RefreshToken
     );
}
