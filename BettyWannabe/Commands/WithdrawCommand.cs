using System;
using BettyWannabe.Interface;
using SharedClasses;

namespace BettyWannabe.Commands
{
	public class WithdrawCommand : ICommand
    {
        private readonly decimal amount;
        private readonly MessagePublisher messagePublisher;

        public WithdrawCommand(decimal amount, MessagePublisher messagePublisher)
		{
            this.amount = amount;
            this.messagePublisher = messagePublisher;
        }

        public async Task ExecuteAsync()
        {
            var walletUpdateMessage = new WalletUpdateMessage()
            {
                Amount = this.amount,
                IsBet = false,
                IsDeposit = false
            };

            await messagePublisher.PublishMessageAsync<WalletUpdateMessage>(walletUpdateMessage, "walletQueue");
        }
    }
}

