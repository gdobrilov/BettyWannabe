//using System;
//using BettyWannabe.Factory;

//namespace BettyWannabe
//{
//    public class CommandHandler
//    {
//        private readonly CommandFactory commandFactory;

//        public CommandHandler(CommandFactory commandFactory)
//        {
//            this.commandFactory = commandFactory;
//        }

//        public void Run()
//        {
//            while (true)
//            {
//                Console.Write("Please submit action:");
//                var input = Console.ReadLine();
//                if (string.IsNullOrWhiteSpace(input)) continue;

//                try
//                {
//                    var command = this.commandFactory.Parse(input);
//                    command.Execute();
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error processing command: {ex.Message}");
//                }
//            }
//        }
//    }
//}

using System;
using System.Threading;
using System.Threading.Tasks;
using BettyWannabe.Factory;
using Microsoft.Extensions.Hosting;
using SharedClasses;

namespace BettyWannabe
{
    public class CommandHandler : BackgroundService
    {
        private readonly CommandFactory _commandFactory;
        private readonly MessageResponseService _messageResponseService;

        public CommandHandler(CommandFactory commandFactory, MessageResponseService messageResponseService)
        {
            _commandFactory = commandFactory;
            _messageResponseService = messageResponseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);

            Console.Write("Please submit action: ");

            while (!stoppingToken.IsCancellationRequested)
            {
                var input = Console.ReadLine()?.Trim();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)) break;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("No input detected, please enter a command.");
                    continue;
                }

                try
                {
                    var command = _commandFactory.Parse(input);
                    await command.ExecuteAsync();

                    if (_messageResponseService.GameOutcomeReceived?.Task != null)
                    {
                        var outcome = await _messageResponseService.GameOutcomeReceived.Task;
                        // Reset or nullify the TaskCompletionSource if necessary to prepare for the next command.
                        _messageResponseService.GameOutcomeReceived = new TaskCompletionSource<string>();
                        Console.Write("Please submit action: ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing command: {ex.Message}");
                    Console.Write("Please submit action: ");
                }

            }
        }
    }
}
