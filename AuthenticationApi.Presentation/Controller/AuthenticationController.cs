using System;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class AuthenticationController : ControllerBase
	{
        private readonly IUser _user;

        public AuthenticationController(IUser user)
		{
			_user = user;
		}

		[HttpPost("register")]
		public async Task<ActionResult<Response>> Register(AppUserDTO dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _user.Register(dto);
			return result.Flag ? Ok(result) : BadRequest(Request);
		}

		[HttpPost("login")]
		public async Task<ActionResult<Response>> Login(LoginDTO dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _user.Login(dto);
			return result.Flag ? Ok(result) : BadRequest(Request);
		}

		[HttpGet("{id:int}")]
		[Authorize]
		public async Task<ActionResult<GetUserDTO>> GetUser(int id)
		{
			if (id <= 0) return BadRequest("Invalid user Id");
			var user = await _user.GetUser(id);
			return user.Id > 0 ? Ok(user) : NotFound(Request);
		}
	}
}

