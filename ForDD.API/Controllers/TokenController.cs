using ForDD.Application.Services;
using ForDD.Domain.Dto;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace ForDD.API.Controllers
{
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto tokenDto)
        {
            var response = await _tokenService.RefreshToken(tokenDto);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
