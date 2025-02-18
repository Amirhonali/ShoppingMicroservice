using System;
using System.Net.Http.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Converstions;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;

namespace OrderApi.Application.Services
{
	public class OrderService : IOrderService
	{
        private readonly IOrder _orderInterface;

		private readonly HttpClient _httpClient;

		private readonly ResiliencePipelineProvider<string> _resilience;

        public OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline)
        {
            _orderInterface = orderInterface;
            _httpClient = httpClient;
            _resilience = resiliencePipeline;
        }

        //GET PRODUCT
        public async Task<ProducDTO> GetProduct(int productId)
        {
            //Call Product Api using HTTPCLIENT
            //Redict this call to the API Geteway since product Api is not response outsiders.
            var getProduct = await _httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;


            var product = await getProduct.Content.ReadFromJsonAsync<ProducDTO>();
            return product!;
        }

        //GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            //Call ProductApi using HTTPCLIENT
            //Redict this call to the API Geteway since product Api is not response outsiders.
            var getUser = await _httpClient.GetAsync($"api/authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        //GET ORDER DETAILS BY ID
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            //Prepare Order
            var order = await _orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
                return null!;

            //Get Retry pipeline
            var retryPipeline = _resilience.GetPipeline("my-retry-pipeline");

            //Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //Prepare Product
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        //GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            //Get all Client's orders
            var orders = await _orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if(!orders.Any()) return null!;

            // Convert from entity to DTO
            var (_, _orders) = OrderConversions.FromEntity(null, orders);
            return _orders!;
        }
    }
}

