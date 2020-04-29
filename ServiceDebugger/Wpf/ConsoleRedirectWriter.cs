using System;
using System.IO;

namespace ServiceDebugger.Wpf
{
    // adapter from https://archive.codeplex.com/?p=consoleredirect
    public class ConsoleRedirectWriter : StringWriter
    {
        private readonly TextWriter _defaultWriter;

        public ConsoleRedirectWriter()
        {
            _defaultWriter = Console.Out;
            OnWrite += text => _defaultWriter.Write(text);
            Console.SetOut(this);
        }

        ~ConsoleRedirectWriter()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            Release();
            base.Dispose(disposing);
        }

        public void Release()
        {
            Console.SetOut(_defaultWriter);
        }

        public override void Write(char value) => WriteGeneric(value);
        public override void Write(string value) => WriteGeneric(value);
        public override void Write(bool value) => WriteGeneric(value);
        public override void Write(int value) => WriteGeneric(value);
        public override void Write(double value) => WriteGeneric(value);
        public override void Write(long value) => WriteGeneric(value);
        public override void WriteLine(char value) => WriteLineGeneric(value);
        public override void WriteLine(string value) => WriteLineGeneric(value);
        public override void WriteLine(bool value) => WriteLineGeneric(value);
        public override void WriteLine(int value) => WriteLineGeneric(value);
        public override void WriteLine(double value) => WriteLineGeneric(value);
        public override void WriteLine(long value) => WriteLineGeneric(value);
        public override void Write(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            var buffer2 = new char[count]; //Ensures large buffers are not a problem
            for (var i = 0; i < count; i++) buffer2[i] = buffer[index + i];
            WriteGeneric(buffer2);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
            var buffer2 = new char[count]; //Ensures large buffers are not a problem
            for (var i = 0; i < count; i++) buffer2[i] = buffer[index + i];
            WriteLineGeneric(buffer2);
        }

        private string WithHour(object message) => $"{DateTime.Now:HH:mm:ss.ff}: {message}";
        private void WriteLineGeneric<T>(T value) => OnWrite?.Invoke($"{WithHour(value)}\n");
        private void WriteGeneric<T>(T value) => OnWrite?.Invoke(WithHour(value));
        public Action<string> OnWrite { get; set; }

    }
}