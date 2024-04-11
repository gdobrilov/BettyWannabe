using System;
using BettyWannabe.Interface;
using SharedClasses;
using SharedClasses.Interface;

namespace BettyWannabe.Commands
{
	public class BetCommand : ICommand
	{
        private readonly decimal amount;
        private readonly IMessagePublisher messagePublisher;

        public BetCommand(decimal amount, IMessagePublisher messagePublisher)
        {
            this.amount = amount;
            this.messagePublisher = messagePublisher;
        }

        public async Task ExecuteAsync()
        {
            if (this.amount <  1 || this.amount > 10)
            {
                throw new ArgumentOutOfRangeException("Bet amount must be between $1 and $10.");
            }

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

