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
        private const int MinBetAmount = 1;
        private const int MaxBetAmount = 1;

        public BetCommand(decimal amount, IMessagePublisher messagePublisher)
        {
            this.amount = amount;
            this.messagePublisher = messagePublisher;
        }

        public async Task ExecuteAsync()
        {
            if (this.amount <  MinBetAmount || this.amount > MaxBetAmount)
            {
                throw new ArgumentOutOfRangeException($"Bet amount must be between ${MinBetAmount} and ${MaxBetAmount}.");
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

