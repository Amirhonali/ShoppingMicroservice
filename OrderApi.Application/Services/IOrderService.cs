using System;
using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Services
{
	public interface IOrderService
	{
		Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId);

		Task<OrderDetailsDTO> GetOrderDetails(int orderId);
	}
}

