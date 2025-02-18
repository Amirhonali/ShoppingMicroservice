using System;
using System.Linq.Expressions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using ProductApi.Presentation.Controllers;
using Xunit;

namespace UnitTest.ProductApi.Repositories
{
	public class ProductRepositoryTest
	{
        private readonly ProductDbContext _dbContext;

        private readonly ProductRepository _repository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb").Options;

            _dbContext = new ProductDbContext(options);

            _repository = new ProductRepository(_dbContext);
        }

        //Add Prodcut
        [Fact]
        public async Task AddAsync_WhenProductAlreadExist_ReturnErrorResponse()
        {
            //Arrange
            var exProduct = new Product { Name = "ExistingProduct" };
            _dbContext.Products.Add(exProduct);
            await _dbContext.SaveChangesAsync();

            //Act
            var result = await _repository.AddAsync(exProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already added");
        }

        [Fact]
        public async Task AddAsync_WhenProductDoesNotExist_AddProductAndReturnsSuccessResponse()
        {
            //Arrage
            var product = new Product() { Name = "New Product" };

            //Act
            var result = await _repository.AddAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product added to database successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnsSuccessResponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 76.93m, Quantity = 5 };
            _dbContext.Products.Add(product);

            //Act
            var result = await _repository.DeleteAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Existing Product is deleted successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnsNotFoundResponse()
        {
            //Arrange
            var product = new Product() { Id = 4, Name = "NonExistingProduct", Price = 76.43m, Quantity = 4 };

            //Act
            var result = await _repository.DeleteAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("NonExistingProduct not found");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnsProduct()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "ExistingProduct", Price = 34.33m, Quantity = 6 };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            //Act
            var result = await _repository.FindByIdAsync(product.Id);

            //Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("ExistingProduct");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNull()
        {
            //Arrange

            //Act
            var result = await _repository.FindByIdAsync(99);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        {
            //Arrange
            var products = new List<Product>()
            {
                new() { Id = 1, Name = "Product 1" },
                new() { Id = 2, Name = "Product 2" }
            };
            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

            //Act
            var result = await _repository.GetAllAync();

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreNotFound_ReturnNull()
        {
            //Arrange

            //Act
            var result = await _repository.GetAllAync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "Product 1" };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 1";

            //Act
            var result = await _repository.GetByIdAsync(predicate);

            //Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            //Arrange
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 2";

            //Act
            var result = await _repository.GetByIdAsync(predicate);

            //Assert
            result.Should().BeNull();

        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsUpdatedSuccessfully_ReturnSuccessResponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "Product 1" };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            //Act
            var result = await _repository.UpdateAsync(product);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 is updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsNotFound_ReturnErrorResponse()
        {
            //Arrange
            var updProduct = new Product() { Id = 1, Name = "Product 22" };


            //Act
            var result = await _repository.UpdateAsync(updProduct);

            //Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 22 not found");
        }
    }
}

