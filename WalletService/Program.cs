using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedClasses;
using SharedClasses.Interface;
using WalletService.Interfaces;
using WalletService.Messaging;
using WalletService.Services;

class Program
{
    public static async Task Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);
        await hostBuilder.RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IWalletService, WalletService.Services.WalletService>();
                services.AddSingleton<IMessagePublisher, MessagePublisher>();
                services.AddHostedService<MessageSubscriber>(serviceProvider =>
                {
                    var walletService = serviceProvider.GetRequiredService<IWalletService>();
                    var messagePublisher = serviceProvider.GetRequiredService<IMessagePublisher>();
                    return new MessageSubscriber(walletService, messagePublisher);
                });
            });
}
