using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.DTOs.User.Requests;

namespace UserManagement.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfirmEmailController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("confirm-email/{token}")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {

          await _mediator.Send(new ConfirmEmailRequest(token));
          return Ok("Email successfully confirmed.");
           
        }
    }
}
