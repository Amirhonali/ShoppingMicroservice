using System;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
	public record OrderDTO(
		int Id,
		[Required, Range(1, int.MaxValue)] int ProductId,
		[Required, Range(1, int.MaxValue)] int ClientId,
		[Required, Range(1, int.MaxValue)] int PurchaseQuantity,
		DateTime OrderDate
		);
}

