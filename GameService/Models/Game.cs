using System;
using GameService.Interface;

namespace GameService.Models
{
	public class Game : IGame
	{
		private readonly Random random = new Random();

		public Tuple<bool, decimal> PlayGame(Bet bet)
		{
            if (bet.Amount < 1 || bet.Amount > 10)
                throw new ArgumentException("Bet amount must be between $1 and $10.");

            double outcomeProbability = this.random.NextDouble();
            return CalculateWinAmount(bet.Amount, outcomeProbability);
		}

        private Tuple<bool, decimal> CalculateWinAmount(decimal betAmount, double outcomeProbability)
        {
            if (outcomeProbability <= 0.5)
            {
                return new Tuple<bool, decimal>(false, 0);
            }
            else if (outcomeProbability <= 0.9)
            {
                decimal winMultiplier = (decimal)(1.0 + this.random.NextDouble());
                winMultiplier = Math.Round(winMultiplier, 2);

                return new Tuple<bool, decimal>(true, betAmount * winMultiplier);
            }
            else
            {
                decimal winMultiplier = (decimal)(2.0 + this.random.NextDouble() * 8.0);
                winMultiplier = Math.Round(winMultiplier, 2);

                return new Tuple<bool, decimal>(true, betAmount * winMultiplier);
            }
        }
    }
}

