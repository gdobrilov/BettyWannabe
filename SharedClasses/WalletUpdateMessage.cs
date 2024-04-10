using System;
namespace SharedClasses
{
	public class WalletUpdateMessage
	{
		public bool IsDeposit { get; set; }
		public bool IsBet { get; set; }
        public decimal Amount { get; set; }
	}
}

