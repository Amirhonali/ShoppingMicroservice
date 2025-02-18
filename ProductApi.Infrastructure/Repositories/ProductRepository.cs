using System;
using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories
{
	public class ProductRepository : IProduct
	{
        private readonly ProductDbContext _dbContext;

		public ProductRepository(ProductDbContext dbContext)
		{
            _dbContext = dbContext;
		}

        public async Task<Response> AddAsync(Product entity)
        {
            try
            {
                //check if the product already exist
                var getProduct = await GetByIdAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already added");

                var currentEntity = _dbContext.Products.Add(entity).Entity;
                await _dbContext.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to database successfully");
                else
                    return new Response(false, $"Error occurred while adding {entity.Name}");
            }
            catch(Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurec adding  new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not found");

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is deleted successfully");
            }
            catch(Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurec deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int Id)
        {
            try
            {
                var products = await _dbContext.Products.FindAsync(Id);

                return products is not null ? products : null!;
            }
            catch(Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new Exception("Error occurred retrieving products");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAync()
        {
            try
            {
                var products = await _dbContext.Products.AsNoTracking().ToListAsync();

                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new InvalidOperationException("Error occurred retrieving products");
            }
        }

        public async Task<Product> GetByIdAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var products = await _dbContext.Products.Where(predicate).FirstOrDefaultAsync();

                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new InvalidOperationException("Error occurred retrieving products");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                {
                    return new Response(false, $"{entity.Name} not found");
                }
                _dbContext.Entry(product).State = EntityState.Detached;
                _dbContext.Products.Update(entity);
                await _dbContext.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                return new Response(false, "Error occurred updating existing product");
            }
        }
    }
}

