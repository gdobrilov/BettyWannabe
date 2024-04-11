using System;
namespace BettyWannabe.Interface
{
	public interface ICommandFactory
	{
        ICommand Parse(string input);
    }
}

