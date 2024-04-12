using System;
using GameService.Models;

namespace GameService.Interface
{
	public interface IGame
	{
        public (bool isWin, decimal amount) PlayGame(Bet bet);
    }
}

