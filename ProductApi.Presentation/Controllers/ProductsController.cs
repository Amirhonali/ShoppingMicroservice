using System;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class ProductsController : ControllerBase
	{
		private readonly IProduct _Interface;

		public ProductsController(IProduct Interface)
		{
			_Interface = Interface;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
		{
			//Get all product from repo
			var products = await _Interface.GetAllAync();
			if (!products.Any())
				return NotFound("No products detected in the database");
			//convert data from entity to DTO and return
			var (_, list) = ProductConversions.FromEntity(null!, products);
			return list!.Any() ? Ok(list) : NotFound("No product found");
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<ProductDTO>> GetProduct(int id)
		{
			//Get single product from th Repo
			var product = await _Interface.FindByIdAsync(id);
			if (product is null)
				return NotFound("Product requested not found");

			//convert from entity to DTO and return 
			var (_product, _) = ProductConversions.FromEntity(product, null!);
			return _product is not null ? Ok(_product) : NotFound("Product not found");
		}

		[HttpPost]
		[Authorize(Roles ="Admin")]
		public async Task<ActionResult<Response>> AddProduct(ProductDTO dto)
		{
			//check model state is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = ProductConversions.ToEntity(dto);
			var response = await _Interface.AddAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpPut]
		[Authorize(Roles ="Admin")]
		public async Task<ActionResult<Response>> UpdateProduct(ProductDTO dto)
		{
			//check modelstate is all data annotations are passed
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			//convert to entity
			var getEntity = ProductConversions.ToEntity(dto);
			var response = await _Interface.UpdateAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}

		[HttpDelete]
		[Authorize(Roles ="Admin")]		
		public async Task<ActionResult<Response>> DeleteProduct(ProductDTO dto)
		{
			//convert to entity
			var getEntity = ProductConversions.ToEntity(dto);
			var response = await _Interface.DeleteAsync(getEntity);
			return response.Flag is true ? Ok(response) : BadRequest(response);
		}
	}
}

