using System;
using SharedClasses;

namespace WalletService.Interfaces
{
	public interface IWalletService
	{
        WalletBalanceUpdateMessage Deposit(WalletUpdateMessage message);

        WalletBalanceUpdateMessage Withdraw(WalletUpdateMessage message);
    }
}

