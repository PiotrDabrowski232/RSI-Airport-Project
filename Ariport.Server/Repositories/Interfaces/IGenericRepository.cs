using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ariport.Server.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(Guid id);
        Task<Guid> Add(T entity);
    }
}