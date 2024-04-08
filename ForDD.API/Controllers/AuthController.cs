using ForDD.Application.Services;
using ForDD.Domain.Dto;
using ForDD.Domain.Dto.User;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace ForDD.API.Controllers
{
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseResult<UserDto>>> Register([FromBody] RegisterUserDto dto)
        {
            var response = await _authService.Register(dto) ;
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResult<TokenDto>>> Login([FromBody]LoginUserDto dto)
        {
            var response = await _authService.Login(dto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
