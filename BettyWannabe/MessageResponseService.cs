using System;
using BettyWannabe.Interface;

namespace BettyWannabe
{
    public class MessageResponseService : IMessageResponseService
    {
        public TaskCompletionSource<string> GameOutcomeReceived { get; set; } = new TaskCompletionSource<string>();
    }
}

