using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository : IUser
    {
        private readonly AuthenticationDbContext _user;

        private readonly IConfiguration _config;

        public UserRepository(AuthenticationDbContext user, IConfiguration config)
        {
            _user = user;
            _config = config;
        }

        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await _user.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user!;
        }

        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await _user.AppUsers.FindAsync(userId);
            return user is not null? new GetUserDTO(user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!) : null!;
        }

        public async Task<Response> Login(LoginDTO dto)
        {
            var getUser = await GetUserByEmail(dto.Email);
            if (getUser is null)
                return new Response(false, "Invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(dto.Password, getUser.Password);
            if (!verifyPassword)
                return new Response(false, "Invalid credentials");

            string token = GenerateToken(getUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!),
            };
            if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                issuer: _config["Authentication:Issuer"],
                audience: _config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentails);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Response> Register(AppUserDTO dto)
        {
            var getUser = await GetUserByEmail(dto.Email);

            if(getUser is not null)
            {
                return new Response(false, $"you cannot use this email for registration");
            }

            var result = _user.AppUsers.Add(new AppUser()
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                TelephoneNumber = dto.TelephoneNumber,
                Address = dto.Address,
                Role = dto.Role
            });

            await _user.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User Regisetered successfuly") :
                new Response(false, "Invalid data provided");
        }
    }
}

