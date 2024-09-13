using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IVillaRepository
    {
        Task Create(Villa Entity);
        Task Update(Villa Entity);
        Task Delete(Villa Entity);
        Task Save();
        Task<List<Villa>> GetAll(Expression<Func<Villa, bool>>? filter = null);
        Task<Villa> Get(Expression<Func<Villa, bool>>? filter = null, bool tracked = true);

    }
}
