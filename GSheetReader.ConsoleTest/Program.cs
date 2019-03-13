using System;

namespace GSheetReader.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GSheetReader.Reader reader = new Reader();
            Console.WriteLine(reader.GetInfo("cafe"));
            Console.WriteLine(reader.GetInfo("filtro"));
            Console.ReadKey();
        }
    }
}
