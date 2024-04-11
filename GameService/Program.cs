using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameService.Messaging;
using GameService.Interface;
using GameService.Models;
using GameService.Core.Services;
using SharedClasses;
using SharedClasses.Interface;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await RunHostAsync(host);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
        services.AddSingleton<IGame, Game>();
        services.AddSingleton<IGameService, GameService.Core.Services.GameService>();
        services.AddHostedService<BetMessageSubscriber>();
    }

    private static async Task RunHostAsync(IHost host)
    {
        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
