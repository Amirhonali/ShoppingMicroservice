using System;
using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interface
{
	public interface IUser
	{
		Task<Response> Register(AppUserDTO dto);

		Task<Response> Login(LoginDTO dto);

		Task<GetUserDTO> GetUser(int userId);
	}
}

