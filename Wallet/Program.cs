using Wallet.Services.Interfaces.Players;
using Wallet.Services.Players;
using Wallet.Services.Interfaces.Transactions;
using Wallet.Services.Transactions;
using Wallet.Domain.Storage;
using Wallet.Repository.Interaces.Players;
using Wallet.Repository.Players;
using Wallet.Repository.Interaces.Transactions;
using Wallet.Repository.Transactions;

namespace Wallet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IGameContext, GameContext>();

            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

            builder.Services.AddScoped<IPlayerService, PlayerService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}