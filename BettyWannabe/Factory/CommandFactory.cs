using System;
using BettyWannabe.Commands;
using BettyWannabe.Interface;
using SharedClasses;
using SharedClasses.Interface;

namespace BettyWannabe.Factory
{
    public class CommandFactory : ICommandFactory
    {
        private IMessagePublisher messagePublisher;

        public CommandFactory(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        private (string command, decimal arg) HandleInput(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException("No command provided");
            }

            var command = parts[0].ToLower();
            decimal arg = parts.Length > 1 ? decimal.Parse(parts[1]) : 0;

            return (command, arg);
        }

        public ICommand Parse(string input)
        {
            (string command, decimal arg) = this.HandleInput(input);

            switch (command)
            {
                case "deposit":
                    return new DepositCommand(arg, this.messagePublisher);
                case "withdraw":
                    return new WithdrawCommand(arg, this.messagePublisher);
                case "bet":
                    return new BetCommand(arg, this.messagePublisher);
                default:
                    throw new ArgumentException($"Unknown command: {command}");
            }
        }
    }
}

