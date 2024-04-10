using System;
using BettyWannabe.Commands;
using BettyWannabe.Interface;
using SharedClasses;

namespace BettyWannabe.Factory
{
    public class CommandFactory
    {
        private MessagePublisher messagePublisher;

        public CommandFactory(MessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        public ICommand Parse(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                throw new ArgumentException("No command provided");
            }

            var command = parts[0].ToLower();
            decimal arg = parts.Length > 1 ? decimal.Parse(parts[1]) : 0;

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

