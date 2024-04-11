using BettyWannabe;
using BettyWannabe.Factory;
using BettyWannabe.Interface;
using BettyWannabe.MessageSubscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedClasses;
using SharedClasses.Interface;
using System;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureServices((hostContext, services) =>
          {
              services.AddSingleton<IMessageResponseService, MessageResponseService>();
              services.AddSingleton<IMessagePublisher, MessagePublisher>();
              services.AddTransient<ICommandFactory, CommandFactory>();
              services.AddSingleton<IConsoleService, ConsoleService>();

              services.AddHostedService<CommandHandler>(serviceProvider =>
              {
                  var commandFactory = serviceProvider.GetRequiredService<ICommandFactory>();
                  var messageResponseService = serviceProvider.GetRequiredService<IMessageResponseService>();
                  var consoleService = serviceProvider.GetRequiredService<IConsoleService>();
                  return new CommandHandler(commandFactory, messageResponseService, consoleService);
              });
              services.AddHostedService<GameMessageSubscriber>();
              services.AddHostedService<WalletMessageSubscriber>();
          });

}
