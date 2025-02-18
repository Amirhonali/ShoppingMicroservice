using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using Xunit;

namespace UnitTest.OrderApi
{
    public class OrderServiceTest
    {
        private readonly IOrderService _service;

        private readonly IOrder _interface;

        public OrderServiceTest()
        {
            _interface = A.Fake<IOrder>();
            _service = A.Fake<IOrderService>();
        }



        //Add Fake HTTP MESSAGE HANDLER
        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response ?? throw new ArgumentNullException(nameof(response));
            }

            protected override Task<HttpResponseMessage> SendAsync
                (HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        //Add Fake HTTP CLIENT USING HTTP FAKE HTTP MESSAGE HANDLER
        private static HttpClient AddFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            return _httpClient;
        }

        //GET PRODUCT
        [Fact]
        public async Task GetProductValidProductIdReturnProduct()
        {
            //Arrange
            int productId = 1;
            var productDTO = new ProducDTO(1, "Product 1", 13, 78.34m);
            var _httpClient = AddFakeHttpClient(productDTO);

            //System Under Test - SUT
            //We Only the httpclient to make calls
            //specify only httpclient and Null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            int productId = 1;
            var _httpClient = AddFakeHttpClient(null!);
            var _orderService = new OrderService(null!, _httpClient, null!);

            //Act
            var result = await _orderService.GetProduct(productId);

            //Assert
            result.Should().BeNull();
        }

        //GET ORDET BY CLIENT ID
        [Fact]
        public async Task GetOrderByClientIs_OrderExist_ReturnOrderDetails()
        {
            //Arrange
            int clientId = 1;
            var orders = new List<Order>
            {
                new() { Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow },
                new() { Id = 2, ProductId = 2, ClientId = clientId, PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow }
            };
            A.CallTo(() => _interface.GetOrdersAsync
            (A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var _orderService = new OrderService(_interface!, null!, null!);

            //Act
            var result = await _orderService.GetOrdersByClientId(clientId);

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(orders.Count);
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}