using System;
using System.Linq.Expressions;
using eCommerce.SharedLibrary.Responses;
namespace eCommerce.SharedLibrary.Interface
{
	public interface IGenericInterface<T> where T : class
	{
		Task<Response> AddAsync(T entity);

        Task<Response> UpdateAsync(T entity);

        Task<Response> DeleteAsync(T entity);

        Task<IEnumerable<T>> GetAllAync();

        Task<T> FindByIdAsync(int Id);

        Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate);

    }
}

