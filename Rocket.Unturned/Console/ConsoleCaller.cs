using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.Unturned.Console
{
    public class ConsoleCaller : IConsoleCommandCaller
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
        public void SendMessage(string message, ConsoleColor? color)
        {
            var tmp = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color ?? tmp;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = tmp;
        }

        public string Name => "Console";
        public Type CallerType => typeof(ConsoleCaller);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return Name.ToString(formatProvider); ;

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return Id.ToString(formatProvider);

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return Name.ToString(formatProvider);

            throw new FormatException($"\"{format}\" is not a valid format.");
        }
    }
}