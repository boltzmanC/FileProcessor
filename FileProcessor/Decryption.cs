using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FileProcessor
{
    public class Decryption
    {

        public void AutoDetectDecrypt()
        {
            string directory = "";



        }



        public void DecryptDirectoryFilesorFile(string enteredpath)
        {
            if (Directory.Exists(enteredpath) == true)
            {



            }
            else if (File.Exists(enteredpath) == true)
            {



            }
            else
            {
                Console.WriteLine($"Entered string, {enteredpath}, is not a valid input.");
                Console.WriteLine();
            }

        }

        public void EncryptDirectoryFilesorFile(string enteredpath)
        {
            //if (Directory.Exists(enteredpath) == true)
            //{

            //}
            //else if (File.Exists(enteredpath) == true)
            //{

            //}
            //else
            //{
            //    Console.WriteLine($"Entered string, {enteredpath}, is not a valid input.");
            //    Console.WriteLine();
            //}
        }

        public void FileDecryptConsole(string filepath)
        {
            // this uses the existing JAVA decryption method. Having these files is a prerequisite to running this program.
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // take in file and get name without pgp extention
            string newfilepath = FunctionTools.GetFileNameWithoutExtension(filepath);

            // run command line commands
            string targusdirectorycheck = @"C:\targus\tds\pgp";

            if (Directory.Exists(targusdirectorycheck))
            {
                string executedecryption = $@"C:\targus\tds\pgp\e1pgp.bat decrypt tdssys {filepath} {newfilepath}";
                Process.Start("CMD.exe", executedecryption);
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@"Directory Not Found: C:\targus\tds\pgp");

                FunctionTools.Exit();
            }

            Console.ResetColor();
        }

        public void FileEncryptConsole(string filepath)
        {
            // this uses the existing JAVA decryption method. Having these files is a prerequisite to running this program.
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // take in file and get name without pgp extention
            string newfilepath = FunctionTools.GetFileNameWithoutExtension(filepath);

            // run command line commands
            string targusdirectorycheck = @"C:\targus\tds\pgp";

            if (Directory.Exists(targusdirectorycheck))
            {
                string executedecryption = $@"C:\targus\tds\pgp\e1pgp.bat encrypt tdssys {filepath} {newfilepath}";
                Process.Start("CMD.exe", executedecryption);
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@"Directory Not Found: C:\targus\tds\pgp");

                FunctionTools.Exit();
            }

            Console.ResetColor();
        }


        //Bouncy Castle (will require no additional software)

        //public void FileDecryptBouncyCastle()
        //{

        //}

        //public void FileEncryptBouncyCastle()
        //{

        //}

    }
}
