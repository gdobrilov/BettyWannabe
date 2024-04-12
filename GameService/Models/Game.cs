using System;
using GameService.Interface;

namespace GameService.Models
{
	public class Game : IGame
	{
		private readonly Random random = new Random();
        private const int MIN_BET_AMOUNT = 1;
        private const int MAX_BET_AMOUNT = 1;
        private const double LOSS_PROBABILITY = 0.5;
        private const double SMALL_WIN_PROBABILITY = 0.9;

        public (bool isWin, decimal amount) PlayGame(Bet bet)
		{
            if (bet.Amount < MIN_BET_AMOUNT || bet.Amount > MAX_BET_AMOUNT)
                throw new ArgumentException($"Bet amount must be between ${MIN_BET_AMOUNT} and ${MAX_BET_AMOUNT}.");

            double outcomeProbability = this.random.NextDouble();
            return CalculateWinAmount(bet.Amount, outcomeProbability);
		}

        private (bool isWin, decimal amount) CalculateWinAmount(decimal betAmount, double outcomeProbability)
        {
            if (outcomeProbability <= LOSS_PROBABILITY)
            {
                return (false, 0);
            }
            else if (outcomeProbability <= SMALL_WIN_PROBABILITY)
            {
                decimal winMultiplier = (decimal)(1.0 + this.random.NextDouble());
                winMultiplier = Math.Round(winMultiplier, 2);

                return (true, betAmount * winMultiplier);
            }
            else
            {
                decimal winMultiplier = (decimal)(2.0 + this.random.NextDouble() * 8.0);
                winMultiplier = Math.Round(winMultiplier, 2);

                return (true, betAmount * winMultiplier);
            }
        }
    }
}

