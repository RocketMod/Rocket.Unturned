using System;
using System.IO;
using System.Text;

namespace Rocket.Unturned.Console
{
    public class UnturnedConsoleWriter : TextWriter
    {
        private readonly TextWriter consoleOutput;
        private readonly TextWriter consoleError;

        private readonly StreamWriter streamWriter;

        public UnturnedConsoleWriter(StreamWriter streamWriter)
        {
            this.streamWriter = streamWriter;
            consoleOutput = System.Console.Out;
            consoleError = System.Console.Error;

            System.Console.SetOut(this);
            System.Console.SetError(this);
        }

        public override Encoding Encoding => consoleOutput.Encoding;
        public override IFormatProvider FormatProvider => consoleOutput.FormatProvider;

        public override string NewLine
        {
            get => consoleOutput.NewLine;
            set => consoleOutput.NewLine = value;
        }

        public override void Close()
        {
            consoleOutput.Close();
            streamWriter.Close();
        }

        public override void Flush()
        {
            consoleOutput.Flush();
            streamWriter.Flush();
        }

        public override void Write(double value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(string value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(object value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(decimal value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(float value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(bool value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(int value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(uint value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(ulong value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(long value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(char[] buffer)
        {
            consoleOutput.Write(buffer);
            streamWriter.Write(buffer);
        }

        public override void Write(char value)
        {
            consoleOutput.Write(value);
            streamWriter.Write(value);
        }

        public override void Write(string format, params object[] arg)
        {
            consoleOutput.Write(format, arg);
            streamWriter.Write(format, arg);
        }

        public override void Write(string format, object arg0)
        {
            consoleOutput.Write(format, arg0);
            streamWriter.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            consoleOutput.Write(format, arg0, arg1);
            streamWriter.Write(format, arg0, arg1);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            consoleOutput.Write(buffer, index, count);
            streamWriter.Write(buffer, index, count);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            consoleOutput.Write(format, arg0, arg1, arg2);
            streamWriter.Write(format, arg0, arg1, arg2);
        }

        public override void WriteLine()
        {
            consoleOutput.WriteLine();
            streamWriter.WriteLine();
        }

        public override void WriteLine(double value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(decimal value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(bool value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            consoleOutput.WriteLine(buffer);
            streamWriter.WriteLine(buffer);
        }

        public override void WriteLine(char value)
        {
            consoleOutput.WriteLine(value);
            streamWriter.WriteLine(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            consoleOutput.WriteLine(format, arg);
            streamWriter.WriteLine(format, arg);
        }

        public override void WriteLine(string format, object arg0)
        {
            consoleOutput.WriteLine(format, arg0);
            streamWriter.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            consoleOutput.WriteLine(format, arg0, arg1);
            streamWriter.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            consoleOutput.WriteLine(buffer, index, count);
            streamWriter.WriteLine(buffer, index, count);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            consoleOutput.WriteLine(format, arg0, arg1, arg2);
            streamWriter.WriteLine(format, arg0, arg1, arg2);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                System.Console.SetOut(consoleOutput);
                System.Console.SetError(consoleError);
            }
        }
    }
}