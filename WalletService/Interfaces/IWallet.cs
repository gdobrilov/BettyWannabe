using System;
namespace WalletService.Interfaces
{
	public interface IWallet
	{
        public bool Withdraw(decimal amount);

        public void Deposit(decimal amount);

        public decimal Balance { get; }
    }
}

