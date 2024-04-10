using System;
using SharedClasses;
using WalletService.Interfaces;
using WalletService.Models;

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

				if (message.IsBet)
				{
                    var response = new WalletBalanceUpdateMessage
                    {
                        Message = $"Congrats - you won {message.Amount}.Your current balance is: ${this.wallet.Balance}",
                        IsSuccessful = true,
                        CurrentBalance = this.wallet.Balance
                    };
                    return response;
				}
                var depositResponse = new WalletBalanceUpdateMessage
                {
                    Message = $"Your deposit of ${message.Amount} was successful. Your current balance is: ${this.wallet.Balance}",
                    IsSuccessful = true,
                    CurrentBalance = this.wallet.Balance
                };
				return depositResponse;
            }
            catch (Exception ex)
			{
                var errorResponse = new WalletBalanceUpdateMessage()
                {
                    IsSuccessful = false,
                    Message = $"The deposit of ${message.Amount} was not successful: {ex.Message}",
                    CurrentBalance = this.wallet.Balance
                };
				return errorResponse;
			}
		}

        public WalletBalanceUpdateMessage Withdraw(WalletUpdateMessage message)
		{
			try
			{
                bool isSuccessful = this.wallet.Withdraw(message.Amount);

                if (isSuccessful)
                {
                    var response = new WalletBalanceUpdateMessage
                    {
                        Message = $"Your withdraw of ${message.Amount} was successful. Your current balance is: ${this.wallet.Balance}",
                        IsSuccessful = true,
                        CurrentBalance = this.wallet.Balance,
                        ShouldPlaceBet = message.IsBet,
                        Amount = message.Amount
                    };
                    return response;
                }

                var errorResponse = new WalletBalanceUpdateMessage
                {
                    Message = $"Your withdraw of ${message.Amount} was not successful due to insufficient balance. Your current balance is: ${this.wallet.Balance}.",
                    IsSuccessful = false,
                    CurrentBalance = this.wallet.Balance
                };
                return errorResponse;
            }
            catch (Exception ex)
			{
                var error = new WalletBalanceUpdateMessage
                {
                    Message = $"The withdraw of ${message.Amount} was not successful: {ex.Message}",
                    IsSuccessful = false,
                    CurrentBalance = this.wallet.Balance
                };
                return error;
            }
        }
    }
}

