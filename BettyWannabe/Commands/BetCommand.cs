using System;
using BettyWannabe.Interface;
using SharedClasses;

namespace BettyWannabe.Commands
{
	public class BetCommand : ICommand
	{
        private readonly decimal amount;
        private readonly MessagePublisher messagePublisher;

        public BetCommand(decimal amount, MessagePublisher messagePublisher)
        {
            this.amount = amount;
            this.messagePublisher = messagePublisher;
        }

        public async Task ExecuteAsync()
        {
            var walletUpdateMessage = new WalletUpdateMessage()
            {
                Amount = this.amount,
                IsBet = true,
                IsDeposit = false,
            };

            await messagePublisher.PublishMessageAsync<WalletUpdateMessage>(walletUpdateMessage, "walletQueue");
        }
    }
}

