using System;
using SharedClasses;

namespace WalletService.Interfaces
{
	public interface IWalletService
	{
        public WalletBalanceUpdateMessage Deposit(WalletUpdateMessage message);

        public WalletBalanceUpdateMessage Withdraw(WalletUpdateMessage amount);
    }
}

