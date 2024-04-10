using System;
namespace BettyWannabe
{
    public class MessageResponseService
    {
        public TaskCompletionSource<string> GameOutcomeReceived { get; set; } = new TaskCompletionSource<string>();
    }
}

