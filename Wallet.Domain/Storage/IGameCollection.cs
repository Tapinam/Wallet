using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Storage
{
    public interface IGameCollection<TModel>
    {
        IEnumerable<TModel> GetAll();
        Task<IEnumerable<TModel>> GetAllAsync();
        TModel Find(Predicate<TModel> predicate);
        Task<TModel> FindAsync(Predicate<TModel> predicate);
        public List<TModel> FindAll(Predicate<TModel> predicate);
        public Task<List<TModel>> FindAllAsync(Predicate<TModel> predicate);
        TModel Add(TModel model);
        Task<TModel> AddAsync(TModel model);
        void Remove(TModel model);
        Task RemoveAsync(TModel model);

        // for test needs
        void AddRange(IEnumerable<TModel> models);
        void Reset();
    }
}
