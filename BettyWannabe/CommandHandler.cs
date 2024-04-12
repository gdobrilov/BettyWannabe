using System;
using System.Threading;
using System.Threading.Tasks;
using BettyWannabe.Factory;
using BettyWannabe.Interface;
using Microsoft.Extensions.Hosting;
using SharedClasses;

namespace BettyWannabe
{
    public class CommandHandler : BackgroundService
    {
        private readonly ICommandFactory commandFactory;
        private readonly IMessageResponseService messageResponseService;
        private readonly IConsoleService consoleService;

        public CommandHandler(ICommandFactory commandFactory, IMessageResponseService messageResponseService, IConsoleService console)
        {
            this.commandFactory = commandFactory;
            this.messageResponseService = messageResponseService;
            this.consoleService = console;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);

            this.consoleService.Write("Please submit action: ");

            while (!stoppingToken.IsCancellationRequested)
            {
                var input = this.consoleService.ReadLine()?.Trim();

                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)) break;

                if (string.IsNullOrWhiteSpace(input))
                {
                    this.consoleService.WriteLine("\nNo input detected, please enter a command.");
                    this.consoleService.Write("Please submit action: ");
                    continue;
                }

                try
                {
                    var command = this.commandFactory.Parse(input);
                    await command.ExecuteAsync();

                    await this.HandleGameOutcomeCompletion();
                }
                catch (Exception ex)
                {
                    this.consoleService.WriteLine(($"Error processing command: {ex.Message}"));
                    this.consoleService.Write("Please submit action: ");
                }

            }
        }

        private async Task HandleGameOutcomeCompletion()
        {
            if (this.messageResponseService.GameOutcomeReceived?.Task != null)
            {
                await this.messageResponseService.GameOutcomeReceived.Task;

                this.messageResponseService.GameOutcomeReceived = new TaskCompletionSource<string>();
                this.consoleService.Write("Please submit action: ");
            }
        }
    }
}