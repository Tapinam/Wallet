using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Domain.Storage;
using Wallet.Repository.Interaces.Generic;

namespace Wallet.Repository.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IGameCollection<T> _collection;

        public GenericRepository(IGameContext gameContext)
        {
            _collection = gameContext.Collection<T>();
        }

        public async Task<T> AddAsync(T gameEntity)
        {
            return await _collection.AddAsync(gameEntity);
        }

        public async Task<List<T>> FindAllAsync(Predicate<T> predicate)
        {
            return await _collection.FindAllAsync(predicate);
        }

        public async Task<T> FindAsync(Predicate<T> predicate)
        {
            return await _collection.FindAsync(predicate);
        }

        public async Task RemoveAsync(T gameEntity)
        {
            await _collection.RemoveAsync(gameEntity);
        }
    }
}
