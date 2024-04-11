using System;
using BettyWannabe.Interface;
using SharedClasses;
using SharedClasses.Interface;

namespace BettyWannabe.Commands
{
	public class WithdrawCommand : ICommand
    {
        private readonly decimal amount;
        private readonly IMessagePublisher messagePublisher;

        public WithdrawCommand(decimal amount, IMessagePublisher messagePublisher)
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

