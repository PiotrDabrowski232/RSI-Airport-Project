using Airport.Server.Context;
using Ariport.Server.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ariport.Server.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AirportDbContext _context;
        public GenericRepository(AirportDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public Task<Guid> Add(T entity)
        {
            return Task.Run(() =>
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
                return (Guid)entity.GetType().GetProperty("Id").GetValue(entity);
            });
        }
    }
}