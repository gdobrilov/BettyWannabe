//using BettyWannabe;
//using BettyWannabe.Factory;
//using BettyWannabe.MessageSubscriber;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using RabbitMQ.Client;
//using SharedClasses;
//using System;
//using System.Text;
//using System.Text.Json;

//class Program
//{
//    public static async Task Main(string[] args)
//    {
//        var host = CreateHostBuilder(args).Build();
//        using var serviceScope = host.Services.CreateScope();
//        var services = serviceScope.ServiceProvider;

//        try
//        {
//            var commandHandler = services.GetRequiredService<CommandHandler>();
//            await commandHandler.RunAsync();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.ToString());
//        }
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//    Host.CreateDefaultBuilder(args)
//        .ConfigureServices((hostContext, services) =>
//        {
//            services.AddSingleton<MessageResponseService>();
//            services.AddSingleton<MessagePublisher>();
//            services.AddTransient<CommandFactory>();
//            services.AddTransient<CommandHandler>();
//            services.AddHostedService<GameMessageSubscriber>();
//            services.AddHostedService<WalletMessageSubscriber>();
//        });
//}

using BettyWannabe;
using BettyWannabe.Factory;
using BettyWannabe.MessageSubscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedClasses;
using System;

class Program
{
    public static async Task Main(string[] args)
    {
        // Create and build the host
        var host = CreateHostBuilder(args).Build();

        // Start the host asynchronously. This automatically starts all registered IHostedService implementations, including your background services.
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureServices((hostContext, services) =>
          {
              services.AddSingleton<MessageResponseService>();
              services.AddSingleton<MessagePublisher>();
              services.AddTransient<CommandFactory>();
              // Register CommandHandler as a hosted service
              services.AddHostedService<CommandHandler>(serviceProvider =>
              {
                  var commandFactory = serviceProvider.GetRequiredService<CommandFactory>();
                  var messageResponseService = serviceProvider.GetRequiredService<MessageResponseService>();
                  return new CommandHandler(commandFactory, messageResponseService);
              });
              services.AddHostedService<GameMessageSubscriber>();
              services.AddHostedService<WalletMessageSubscriber>();
          });

}
