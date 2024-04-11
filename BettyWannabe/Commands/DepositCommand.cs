using System;
using System.Threading.Tasks;
using BettyWannabe.Interface;
using SharedClasses;
using SharedClasses.Interface;

namespace BettyWannabe.Commands
{
    public class DepositCommand : ICommand
    {
        private readonly decimal amount;
        private readonly IMessagePublisher messagePublisher;

        public DepositCommand(decimal amount, IMessagePublisher messagePublisher)
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
                IsDeposit = true
            };

            await messagePublisher.PublishMessageAsync<WalletUpdateMessage>(walletUpdateMessage, "walletQueue");
        }
    }
}
