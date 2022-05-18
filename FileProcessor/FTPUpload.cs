using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinSCP;
using System.IO;

namespace FileProcessor
{
    public class FTPUpload
    {

        // pick option from main menu
        // pick NIGHTLY or NOW
        // open menu for file to transfer.
        // pick hh file to move to ftp
        // name change file to match definition file. 

        public static void ChooseFileToUpload ()
        {
            // get target file to transfer
            string targetfile = FunctionTools.GetAFile();

            string targetfileextension = Path.GetExtension(targetfile);

            // get name + path for all definition files.
            Dictionary<string, string> definitionfilelookup = FunctionTools.GetListofDefinitionFiles(typeof(DefinitionFileList).GetAllPublicConstantValues<string>());

            // select definition file path.
            string targetdefinitionfile = ChooseDefinitionFilePath(definitionfilelookup);

            //get path of defintion file
            string[] splitfilepath = targetdefinitionfile.Split('/');
            splitfilepath = splitfilepath.Take(splitfilepath.Length - 1).ToArray(); // remove last entry. which is the definition file name.

            string destinationpath = string.Join("/", splitfilepath);

            // now or nightly
            //
            //
            //
            //
            //
            //


            //create new name for file to have after transfer.
            string newfilename = FunctionTools.GetFileNameWithoutExtension(targetdefinitionfile);

            string buildnewfilename = destinationpath + "/" + newfilename + targetfileextension;


            using (Session session = new Session())
            {
                // Connect
                int attempts = 3;
                do
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Connecting to download.targusinfo.com:");
                        session.Open(FTPLogins.E1Platform());
                        Console.WriteLine("Connected.");
                        Console.ResetColor();
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"Failed to connect - {e}");
                        if (attempts == 0)
                        {
                            // give up
                            Console.WriteLine("I give up...");
                            throw;
                        }
                        Console.ResetColor();
                    }
                    attempts--;
                }
                while (!session.Opened);

                try
                {
                    
                    session.PutFiles(targetfile, buildnewfilename);

                    Console.WriteLine($"File transfered: {buildnewfilename}");
                }
                catch (WinSCP.SessionException error)
                {
                    Console.WriteLine($"SessionExceptionError: {error}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }




        static string ChooseDefinitionFilePath (Dictionary<string, string> definitionfilelookup)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select file to upload to the e1platform FTP: ");

            int filecount = definitionfilelookup.Count();

            for (int x = 0; x < filecount; x++)
            {
                Console.WriteLine("{0,5}{1,-10}", x + ". ", definitionfilelookup.ElementAt(x).Key);
            }

            Console.WriteLine("{0,5}{1,-10}", "exit: ", "exit");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Definition File: ");
            string input = Console.ReadLine().ToUpper();

            if (input.ToLower() == "exit") //handle exit option
            {
                Processor.ProcessorStartMenu();
            }

            string definitionfilepath = string.Empty;

            int index;

            // parse input
            bool parsed = Int32.TryParse(input, out index);
            if (parsed && index < filecount)
            {
                definitionfilepath = definitionfilelookup.ElementAt(index).Value;
            }
            else
            {
                Console.WriteLine("Invalid number entered, try again...");
                //E1PlatformFileTester(); //restart.
                ChooseDefinitionFilePath(definitionfilelookup); // restart same menu.
            }

            return definitionfilepath;
        }


    }
}
