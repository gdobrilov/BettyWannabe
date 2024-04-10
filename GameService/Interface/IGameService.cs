using System;
using SharedClasses;

namespace GameService.Interface
{
	public interface IGameService
	{
		Task<WalletUpdateMessage> PlayBetAsync(BetMessage betMessage);
    }
}