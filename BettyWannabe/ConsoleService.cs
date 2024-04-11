using System;
using BettyWannabe.Interface;

namespace BettyWannabe
{
    public class ConsoleService : IConsoleService
    {
        public string ReadLine() => Console.ReadLine();
        public void WriteLine(string message) => Console.WriteLine(message);
        public void Write(string message) => Console.Write(message);
    }

}

