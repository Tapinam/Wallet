using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Storage
{
    internal sealed class Game : IDisposable
    {
        private readonly IDictionary<Type, object> _data;
        private static readonly Lazy<Game> _game = new Lazy<Game>(() => new Game());

        internal static Game Instance { get; private set; } = _game.Value;

        private Game()
        {
            _data = new Dictionary<Type, object>();
        }


        internal GameCollection<TModel> GetCollection<TModel>()
        {
            var found = _data.TryGetValue(typeof(TModel), out var collection);

            if(!found)
            {
                throw new InvalidOperationException("Collection not found");
            }

            return collection as GameCollection<TModel>;
        }

        internal void SetCollection<TModel>(GameCollection<TModel> collection)
        {
            if(!_data.ContainsKey(typeof(TModel)))
                _data.Add(typeof(TModel), collection);
        }

        public void Dispose()
        {
            _data.Clear();
        }
    }
}
