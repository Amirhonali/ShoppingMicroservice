using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Converstions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;


namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrder _Interface;

        private readonly IOrderService _service;

        public OrdersController(IOrder Interface, IOrderService service)
        {
            _Interface = Interface;
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllAsync()
        {
            var orders = await _Interface.GetAllAync();
            if (!orders.Any())
                return NotFound("No order detected in the database");

            var (_, list) = OrderConversions.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _Interface.FindByIdAsync(id);
            if (order is null)
                return NotFound(null);

            var (_order, _) = OrderConversions.FromEntity(order, null);
            return Ok(order);

        }

        [HttpGet("client/{clientId:int}")]

        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return BadRequest("Invalid data provided");

            var orders = await _Interface.GetOrdersAsync(o => o.ClientId == clientId);
            return !orders.Any() ? NotFound(null) : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid data provided");
            var orderDetails = await _service.GetOrderDetails(orderId);
            return orderDetails.OrderId > 0 ? Ok(orderDetails) : NotFound("No order found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> AddOrder(OrderDTO dto)
        {
            //Check model state if all data annotations are passed.
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");

            //convert to entity
            var getEntity = OrderConversions.ToEntity(dto);
            var response = await _Interface.AddAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO dto)
        {
            //convert from dto to entity
            var order = OrderConversions.ToEntity(dto);
            var response = await _Interface.UpdateAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> Delete(OrderDTO dto)
        {
            var order = OrderConversions.ToEntity(dto);
            var response = await _Interface.DeleteAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}

