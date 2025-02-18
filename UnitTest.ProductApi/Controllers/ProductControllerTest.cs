using System;
using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;
using Xunit;

namespace UnitTest.ProductApi.Controllers
{
	public class ProductControllerTest
	{
		private readonly IProduct _Interface;

		private readonly ProductsController _controller;

		public ProductControllerTest()
		{
			//Set up dependenies
			_Interface = A.Fake<IProduct>();

			//Set up System Under Test - SUT
			_controller = new ProductsController(_Interface);
		}

		//GET All Products
		[Fact]
		public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProduct()
		{
			//Arrange
			var products = new List<Product>()
			{
				new() { Id = 1, Name = "Product 1", Quantity = 10, Price = 100.70m},
                new() { Id = 2, Name = "Product 2", Quantity = 110, Price = 1004.70m}
            };

			//set up fake response for GetAllAsync method
			A.CallTo(() => _Interface.GetAllAync()).Returns(products);

			//Act
			var result = await _controller.GetProducts();

			//Assert
			var okResult = result.Result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

			var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
			returnedProducts.Should().NotBeNull();
			returnedProducts.Should().HaveCount(2);
			returnedProducts!.First().Id.Should().Be(1);
			returnedProducts!.Last().Id.Should().Be(2);
		}

		[Fact]
		public async Task GetProducts_WhenNoProductsExist_ReturnNotFoundResponse()
		{
			//Arrange
			var products = new List<Product>();

			//Set up fake response for GetAllAsync();
			A.CallTo(() => _Interface.GetAllAync()).Returns(products);

			//Act
			var result = await _controller.GetProducts();

			//Assert
			var notFoundResult = result.Result as NotFoundObjectResult;
			notFoundResult.Should().NotBeNull();
			notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

			var message = notFoundResult.Value as string;
			message.Should().Be("No products detected in the database");
		}

		//Add product 
		[Fact]
		public async Task AddProduct_WhenModelStateIsInvalid_ReturBadRequest()
		{
			//Arrange
			var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
			_controller.ModelState.AddModelError("Name", "Required");

			//Act

			var result = await _controller.AddProduct(productDTO);

			//Assert
			var badRequestResult = result.Result as BadRequestObjectResult;
			badRequestResult.Should().NotBeNull();
			badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		}

		[Fact]
		public async Task AddProduct_WhenAddIsSuccessful_ReturnOkResponse()
		{
			//Arrange
			var productDTO = new ProductDTO(1, "Product 1 ", 34, 67.95m);
			var response = new Response(true, "Added");

			//Act
			A.CallTo(() => _Interface.AddAsync(A<Product>.Ignored)).Returns(response);
			var result = await _controller.AddProduct(productDTO);

			//Assert
			var okResult = result.Result as OkObjectResult;
			okResult!.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

			var responseResult = okResult.Value as Response;
			responseResult!.Message.Should().Be("Added");
			responseResult!.Flag.Should().BeTrue();
		}

		[Fact]
		public async Task AddProduct_WhenAddFails_ReturnBadRequestResponse()
		{
			//Arrange
			var productDTO = new ProductDTO(1, "Product 1", 78, 45.89m);
			var response = new Response(false, "Failed");

			//Act
			A.CallTo(() => _Interface.AddAsync(A<Product>.Ignored)).Returns(response);
			var result = await _controller.AddProduct(productDTO);

			//Assert
			var badRequestResult = result.Result as BadRequestObjectResult;
			badRequestResult.Should().NotBeNull();
			badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

			var responseResult = badRequestResult.Value as Response;
			responseResult.Should().NotBeNull();
			responseResult!.Message.Should().Be("Failed");
			responseResult!.Flag.Should().BeFalse();
		}

		[Fact]
		public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnOkResponse()
		{
			//Arrange
			var productDTO = new ProductDTO(1, "Product 1", 78, 74.49m);
			var response = new Response(true, "Update");

			//Act
			A.CallTo(() => _Interface.UpdateAsync(A<Product>.Ignored)).Returns(response);
			var result = await _controller.UpdateProduct(productDTO);

			//Assert
			var okResult = result.Result as OkObjectResult;
			okResult!.Should().NotBeNull();
			okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

			var responseResult = okResult.Value as Response;
			responseResult!.Message.Should().Be("Update");
			responseResult!.Flag.Should().BeTrue();
		}

		[Fact]
		public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
		{
			//Arrange
			var productDTO = new ProductDTO(1, "Product 1", 83, 89.09m);
			var response = new Response(false, "Update Failed");

			//Act
			A.CallTo(() => _Interface.UpdateAsync(A<Product>.Ignored)).Returns(response);
			var result = await _controller.UpdateProduct(productDTO);

			//Assert
			var badRequestResult = result.Result as BadRequestObjectResult;
			badRequestResult.Should().NotBeNull();
			badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

			var responseResult = badRequestResult.Value as Response;
			responseResult.Should().NotBeNull();
			responseResult!.Message.Should().Be("Update Failed");
			responseResult!.Flag.Should().BeFalse();
		}

		[Fact]
        public async Task DeleteProduct_WhenUpdateIsSuccessful_ReturnOkResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 78, 74.49m);
            var response = new Response(true, "Deleted Successfully");

            //Act
            A.CallTo(() => _Interface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await _controller.DeleteProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult!.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Message.Should().Be("Deleted Successfully");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenUpdateFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 83, 89.09m);
            var response = new Response(false, "Delete Failed");

            //Act
            A.CallTo(() => _Interface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await _controller.DeleteProduct(productDTO);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult!.Message.Should().Be("Delete Failed");
            responseResult!.Flag.Should().BeFalse();
        }
    }
}

