using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;

namespace UserManagement.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
            => Ok(await _mediator.Send(request));


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
            => Ok(await _mediator.Send(request));


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _mediator.Send(new UserLogoutRequest());
            return NoContent();
        }

        [HttpPost("refresh-token")]

        public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenRequest request)
        {
            var userTokenResponse = await _mediator.Send(request);
            return Ok(userTokenResponse);
        }

        [HttpPut("deactivate")]
        public async Task<IActionResult> DeactivateUser()
        {
            await _mediator.Send(new DeactivateUserRequest());
            return Ok("User deactivated successfully.");
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var request = new CurrentUserRequest();
            var userResponse = await _mediator.Send(request);
            return Ok(userResponse);
        }

        [HttpPost("send-recovery-email")]
        public async Task<IActionResult> SendRecoveryEmail([FromBody] SendAccountTokenRequest request)
        {
            await _mediator.Send(request);
            return Ok("Account recovery email sent successfully.");
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverAccount([FromQuery] string token)
        {
            await _mediator.Send(new RecoverAccountRequest(token));
            return Ok("User deactivated successfully.");
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
        {
            var users = await _mediator.Send(new GetAllUsersRequest());
            return Ok(users);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            await _mediator.Send(request);
            return Ok(new { Message = "User updated successfully" });
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var request = new DeleteUserRequest(id);
            await _mediator.Send(request);
            return NoContent();
        }

        [HttpGet("{userId}/status")]
        public async Task<IActionResult> GetUserStatus(Guid userId)
        {
            var status = await _mediator.Send(new UserStatusRequest(userId));
            return Ok(status);
        }
    }
}
