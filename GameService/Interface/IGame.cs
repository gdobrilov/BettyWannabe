using System;
using GameService.Models;

namespace GameService.Interface
{
	public interface IGame
	{
        public Tuple<bool, decimal> PlayGame(Bet bet);
    }
}

