using System;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
    public static class ConsoleHelpers
    {
        public static void WriteTitle(string text)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("*** " + text + " ***");
            Console.ResetColor();
        }

        public static void WriteSubTitle(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("    " + text);
            Console.ResetColor();
        }

        public static void WriteTextWith1Indent(string text)
        {
            Console.WriteLine("    " + text);
        }

        public static void WriteTextWith2Indent(string text)
        {
            Console.WriteLine("        " + text);
        }
        public static void WriteTextReplacingLastLine(string text)
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.Write($"\r {text}");
        }
    }
}
