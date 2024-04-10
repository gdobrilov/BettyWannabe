using System;
using WalletService.Interfaces;

namespace WalletService.Models
{
	public class Wallet : IWallet
	{
		public decimal Balance { get; private set; }

		public Wallet(decimal initialBalance = 0)
		{
			this.Balance = initialBalance;
		}

		public void Deposit(decimal amount)
		{
			if (amount <= 0)
			{
				throw new ArgumentOutOfRangeException("Deposit amount must be greater than zero.");
			}

			this.Balance += amount;
		}

		public bool Withdraw(decimal amount)
		{
            if (amount <= 0)
			{
                throw new ArgumentOutOfRangeException("Withdraw amount must be greater than zero.");
            }

			if (this.Balance >= amount)
			{
				Balance -= amount;
				return true;
			}

			return false;
		}
	}
}