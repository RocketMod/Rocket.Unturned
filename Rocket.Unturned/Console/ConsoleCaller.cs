using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Unturned.Console
{
    public class ConsoleCaller : ICommandCaller //Todo: IConsoleCommandCaller
    {
        private static ConsoleCaller instance;
        public static ConsoleCaller Instance
        {
            get { return instance ?? (instance = new ConsoleCaller()); }
        }

        public int CompareTo(object obj)
        {
            return -((ICommandCaller)obj).CompareTo(instance);
        }

        public int CompareTo(IIdentifiable other)
        {
            return Id.CompareTo(other.Id);
        }

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;
            return Id.Equals(other.Id);
        }

        public int CompareTo(string other)
        {
            return Id.CompareTo(other);
        }

        public bool Equals(string other)
        {
            return Id.Equals(other);
        }

        public string Id => "Console";
        public void SendMessage(string message)
        {
            //Todo use logger
            System.Console.WriteLine(message);
        }

        public string Name => "Console";
        public Type PlayerType => typeof(ConsoleCaller);
    }
}