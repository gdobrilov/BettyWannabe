using System;
using System.Threading.Tasks;
using WalletService.Interfaces;
using WalletService.Models;
using SharedClasses;

namespace WalletService.Services
{
    public class WalletService : IWalletService
    {
        private Wallet wallet;

        public WalletService()
        {
            this.wallet = new Wallet();
        }

        public WalletBalanceUpdateMessage Deposit(WalletUpdateMessage message)
        {
            try
            {
                this.wallet.Deposit(message.Amount);

                var response = new WalletBalanceUpdateMessage
                {
                    Message = message.IsBet ?
                        $"Congrats - you won {message.Amount}. Your current balance is: ${this.wallet.Balance}" :
                        $"Your deposit of ${message.Amount} was successful. Your current balance is: ${this.wallet.Balance}",
                    IsSuccessful = true,
                    CurrentBalance = this.wallet.Balance
                };

                return response;
            }
            catch (Exception ex)
            {
                return new WalletBalanceUpdateMessage
                {
                    IsSuccessful = false,
                    Message = $"The deposit of ${message.Amount} was not successful: {ex.Message}",
                    CurrentBalance = this.wallet.Balance
                };
            }
        }

        public WalletBalanceUpdateMessage Withdraw(WalletUpdateMessage message)
        {
            try
            {
                bool isSuccessful = this.wallet.Withdraw(message.Amount);

                var response = new WalletBalanceUpdateMessage
                {
                    Message = isSuccessful ?
                        $"Your withdraw of ${message.Amount} was successful. Your current balance is: ${this.wallet.Balance}" :
                        $"Your withdraw of ${message.Amount} was not successful due to insufficient balance. Your current balance is: ${this.wallet.Balance}.",
                    IsSuccessful = isSuccessful,
                    CurrentBalance = this.wallet.Balance,
                    ShouldPlaceBet = message.IsBet,
                    Amount = message.Amount
                };

                return response;
            }
            catch (Exception ex)
            {
                return new WalletBalanceUpdateMessage
                {
                    Message = $"The withdraw of ${message.Amount} was not successful: {ex.Message}",
                    IsSuccessful = false,
                    CurrentBalance = this.wallet.Balance
                };
            }
        }
    }
}
