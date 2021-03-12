using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileProcessor
{
    class Processor
    {
        static void Main(string[] args)
        {
            //what would you like to do.

            Console.WriteLine();
            Console.WriteLine("Select a process:");
            Console.WriteLine("{0,5}{1,-10}", "1. ", "Decrypt file or files in target directory.");
            Console.WriteLine("{0,5}{1,-10}", "2. ", "Test file format against definition file.");
            Console.WriteLine("{0,5}{1,-10}", "3. ", "");
            Console.WriteLine("{0,5}{1,-10}", "4. ", "");
            Console.WriteLine("{0,5}{1,-10}", "5. ", "");

            Console.WriteLine("{0,5}", "exit");

            Console.WriteLine();
            Console.Write("Selection: ");
            string input = Console.ReadLine().ToUpper();

            string definitionfilepath = string.Empty;

            switch (input)
            {
                case "1":
                    
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
