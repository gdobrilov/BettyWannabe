using System;
namespace SharedClasses.Interface
{
	public interface IMessagePublisher
	{
        Task PublishMessageAsync<T>(T message, string queueName);
    }
}

