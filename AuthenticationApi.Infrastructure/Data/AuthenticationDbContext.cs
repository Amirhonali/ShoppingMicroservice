using System;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
	public class AuthenticationDbContext : DbContext
	{
		public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options) {}

		public DbSet<AppUser> AppUsers { get; set; }
	}
}

