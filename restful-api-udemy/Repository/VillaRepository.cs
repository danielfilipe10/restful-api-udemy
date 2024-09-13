using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _context;

        public VillaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Create(Villa entity)
        {
            await _context.AddAsync(entity);
            await Save();
        }

        public async Task Delete(Villa entity)
        {
            _context.Remove(entity);
            await Save();
        }

        public async Task<Villa> Get(Expression<Func<Villa, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<Villa> query = _context.Villas;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Villa>> GetAll(Expression<Func<Villa, bool>>? filter = null)
        {
            IQueryable<Villa> query = _context.Villas;

            if(filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
            
        }

        public Task Update(Villa entity)
        {
            throw new NotImplementedException();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
