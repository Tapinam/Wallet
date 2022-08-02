using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Storage
{
    public class GameCollection<TModel> : IEnumerable<TModel>, IGameCollection<TModel>
    {
        private readonly List<TModel> _items;

        public GameCollection()
        {
            _items = new List<TModel>();
        }

        public TModel Add(TModel model)
        {
            _items.Add(model);
            return model;
        }

        public async Task<TModel> AddAsync(TModel model)
        {
            return await Task.Run(() =>
            {
                return this.Add(model);
            });
        }

        public void Remove(TModel model)
        {
            _items.Remove(model);
        }

        public async Task RemoveAsync(TModel model)
        {
            await Task.Run(() =>
            {
                this.Remove(model);
            });
        }

        public void AddRange(IEnumerable<TModel> models)
        {
            _items.AddRange(models);
        }

        public TModel Find(Predicate<TModel> predicate)
        {
            return _items.Find(predicate ?? ((item) => false));
        }

        public async Task<TModel> FindAsync(Predicate<TModel> predicate)
        {
            return await Task.Run(() =>
            {
                return this.Find(predicate);
            });
        }
        public List<TModel> FindAll(Predicate<TModel> predicate)
        {
            return _items.FindAll(predicate ?? ((item) => false));
        }

        public async Task<List<TModel>> FindAllAsync(Predicate<TModel> predicate)
        {
            return await Task.Run(() =>
            {
                return this.FindAll(predicate);
            });
        }

        public IEnumerable<TModel> GetAll()
        {
            return _items;
        }

        public void Reset()
        {
            _items.Clear();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                return this.GetAll();
            });
        }

        public IEnumerator<TModel> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
