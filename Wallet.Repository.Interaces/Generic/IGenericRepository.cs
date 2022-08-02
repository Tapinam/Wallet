using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Repository.Interaces.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> FindAsync(Predicate<T> predicate);
        Task<List<T>> FindAllAsync(Predicate<T> predicate);
        Task<T> AddAsync(T gameEntity);
        Task RemoveAsync(T gameEntity);
    }
}
