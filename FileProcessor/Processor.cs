using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileProcessor
{
    public class Processor
    {
        public static void Main(string[] args)
        {
            FunctionTools.Introduction();
            FunctionTools.ConsoleSize();


            //GO
            ProcessorStartMenu();

            //sweeper
            //Sweeper.OnBoardingFTPSweeper();
            //Sweeper.OnboardingFTPWriteFileListToConsole();
        }

        public static void ProcessorStartMenu()
        {
            // start menu
            bool done = false;

            while (done != true)
            {
                //what would you like to do.

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("Select a process:");
                Console.WriteLine("{0,5}{1,-10}", "1. ", "Open HowTO Confluence Page.");
                Console.WriteLine("{0,5}{1,-10}", "2. ", "Decrypt file or files in target directory.");
                Console.WriteLine("{0,5}{1,-10}", "3. ", "Test file format against definition file.");
                Console.WriteLine("{0,5}{1,-10}", "4. ", "Generate new .definition file.");
                Console.WriteLine("{0,5}{1,-10}", "5. ", "Get subset of records");
                Console.WriteLine("{0,5}{1,-10}", "6. ", "Onboarding Sweeper.");
                //Console.WriteLine("{0,5}{1,-10}", "7. ", "");
                //Console.WriteLine("{0,5}{1,-10}", "8. ", "");
                Console.WriteLine("{0,5}{1,-10}", "exit. ", "End Program.");
                Console.WriteLine();
                Console.ResetColor();
                
                //Get user input.
                Console.Write("Selection: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        //System.Diagnostics.Process.Start("insert webpage url here");
                        Console.WriteLine("Not implemented yet.");
                        break;

                    case "2":
                        Decryption.DecryptFileStart();
                        break;

                    case "3":
                        DefinitionFileTester.E1PlatformFileTester();
                        break;

                    case "4":
                        DefinitionFileTester.GenerateNewDefinitionFile();
                        break;

                    case "5":
                        DefinitionFileTester.GetSubsetOfRecordsStandAlone();
                        break;

                    case "6":
                        Sweeper.OnBoardingFTPSweeper();
                        break;


                    // exit
                    case "exit":
                        done = true; // we are done but manually exit app here.
                        FunctionTools.ExitApp();
                        break;

                    default:
                        Console.WriteLine("not a valid input");
                        ProcessorStartMenu();
                        break;

                }
            }
        }
    }

}
