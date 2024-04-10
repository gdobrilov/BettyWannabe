using System;
namespace BettyWannabe.Interface
{
	public interface ICommand
	{
        Task ExecuteAsync();
    }
}

