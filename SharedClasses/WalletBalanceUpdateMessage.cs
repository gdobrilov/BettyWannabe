using System;
namespace SharedClasses
{
	public class WalletBalanceUpdateMessage
	{
		public bool IsSuccessful { get; set; }
		public string Message { get; set; } = "";
		public decimal CurrentBalance { get; set; }
		public bool ShouldPlaceBet { get; set; }
		public decimal Amount { get; set; }
    }
}

