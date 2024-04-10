using System;
using GameService.Interface;

namespace GameService.Models
{
	public class Bet : IBet
	{
		private decimal amount;

		public Bet(decimal amount)
		{
			this.Amount = amount;
		}

		public decimal Amount {
			get
			{
				return amount;
			}
			set
			{
				if (value < 1 && value > 10)
				{
					throw new ArgumentOutOfRangeException("Amount should be between 1 and 10");
				}

				amount = value;
			}
		}

		public decimal Win { get; set; } = 0;

    }
}

