using System;
namespace BettyWannabe.Interface
{
	public interface IMessageResponseService
	{
        TaskCompletionSource<string> GameOutcomeReceived { get; set; }
    }
}

