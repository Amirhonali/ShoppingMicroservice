using System;
using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;

namespace OrderApi.Infrastructure.Data
{
	public class OrderDbContext : DbContext
	{
		public OrderDbContext(DbContextOptions<OrderDbContext> options)
			:base(options)
		{
		}

        public DbSet<Order> Orders { get; set; }

    }
}

