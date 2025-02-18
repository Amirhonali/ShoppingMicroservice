using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories
{
	public class OrderRepository : IOrder
    {
        private readonly OrderDbContext _dbContext;

        public OrderRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Response> AddAsync(Order entity)
        {
            try
            {
                var order = _dbContext.Orders.Add(entity).Entity;
                await _dbContext.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order placed successfully") :
                    new Response(false, "Error occured while placing order");
            }
            catch(Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                _dbContext.Orders.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new Response(true, "Order successfully deleted");
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                return new Response(false, "Error occured while deleting order");
            }
        }

        public async Task<Order> FindByIdAsync(int Id)
        {
            try
            {
                var order = await _dbContext.Orders!.FindAsync(Id);
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAync()
        {
            try
            {
                var orders = await _dbContext.Orders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<Order> GetByIdAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await _dbContext.Orders.Where(predicate).FirstOrDefaultAsync();
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await _dbContext.Orders.Where(predicate).ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                    return new Response(false, $"Order not found");

                _dbContext.Entry(order).State = EntityState.Detached;
                _dbContext.Orders.Update(entity);
                await _dbContext.SaveChangesAsync();
                return new Response(true, "Order updated");
            }
            catch (Exception ex)
            {
                //Log Original Exception
                LogException.LogExceptions(ex);

                //Display scary-free message to client
                return new Response(false, "Error occured while placing order");
            }
        }
    }
}

