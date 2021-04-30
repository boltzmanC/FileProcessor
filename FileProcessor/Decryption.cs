using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using WinSCP;

namespace FileProcessor
{
    public class Decryption
    {
        public static void DecryptFileStart()
        {
            string file = FunctionTools.GetAFile();
            SameDirectoryFileDecryptConsole(file);
        }


        public static void SameDirectoryFileDecryptConsole(string filepath)
        {
            // this uses the existing JAVA decryption method. Having these files is a prerequisite to running this program.
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // take in file and get name without pgp extention
            string parentdirectory = FunctionTools.GetParentFolder(filepath);
            string newfilepath = FunctionTools.GetFileNameWithoutExtension(filepath);
            newfilepath = parentdirectory + @"\" + newfilepath;

            // run command line commands
            string targusdirectorycheck = @"C:\targus\tds\pgp";

            if (Directory.Exists(targusdirectorycheck))
            {
                string executedecryption = String.Format($@"C:\targus\tds\pgp\e1pgp.bat decrypt tdssys {filepath} {newfilepath}");

                try
                {
                    // define command
                    Process decryptcommand = new Process();
                    decryptcommand.StartInfo.FileName = "cmd.exe";
                    decryptcommand.StartInfo.Arguments = "/c " + executedecryption;
                    decryptcommand.StartInfo.UseShellExecute = false;
                    decryptcommand.StartInfo.CreateNoWindow = true;
                    decryptcommand.StartInfo.RedirectStandardOutput = true;
                    decryptcommand.StartInfo.RedirectStandardError = true;
                    //decryptcommand.StartInfo.RedirectStandardInput = true;

                    decryptcommand.Start();

                    //write output to console
                    while (!decryptcommand.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(decryptcommand.StandardOutput.ReadLine());
                    }

                    //write to console in one block
                    //string output = decryptcommand.StandardOutput.ReadToEnd();
                    //Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }                
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@"Directory Not Found: C:\targus\tds\pgp");
            }

            Console.ResetColor();
        }

        public static void SameDirectoryFileEncryptConsole(string filepath)
        {
            // this uses the existing JAVA encryption method. Having these files is a prerequisite to running this program.
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            // take in file and get name without pgp extention
            string parentdirectory = FunctionTools.GetParentFolder(filepath);
            string newfilepath = FunctionTools.GetFileNameWithoutExtension(filepath);
            newfilepath = parentdirectory + @"\" + newfilepath;

            // run command line commands
            string targusdirectorycheck = @"C:\targus\tds\pgp";

            if (Directory.Exists(targusdirectorycheck))
            {
                string executeencryption = String.Format($@"C:\targus\tds\pgp\e1pgp.bat decrypt tdssys {filepath} {newfilepath}");

                try
                {
                    // define command
                    Process decryptcommand = new Process();
                    decryptcommand.StartInfo.FileName = "cmd.exe";
                    decryptcommand.StartInfo.Arguments = "/c " + executeencryption;
                    decryptcommand.StartInfo.UseShellExecute = false;
                    decryptcommand.StartInfo.CreateNoWindow = true;
                    decryptcommand.StartInfo.RedirectStandardOutput = true;
                    decryptcommand.StartInfo.RedirectStandardError = true;
                    //decryptcommand.StartInfo.RedirectStandardInput = true;

                    decryptcommand.Start();

                    //write output to console
                    while (!decryptcommand.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(decryptcommand.StandardOutput.ReadLine());
                    }

                    //write to console in one block
                    //string output = decryptcommand.StandardOutput.ReadToEnd();
                    //Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@"Directory Not Found: C:\targus\tds\pgp");
            }

            Console.ResetColor();
        }

        public static void SweeperFileDownloadAndDecryptConsole(Session session, string ftpfilepath, string targetdirectory)
        {
            // this uses the existing JAVA decryption method. Having these files is a prerequisite to running this program.
            Console.WriteLine();
            Console.WriteLine($"Downloading: {ftpfilepath}");
            

            // download file and store on target drive / directory.
            session.GetFileToDirectory(ftpfilepath, targetdirectory, false); //https://winscp.net/eng/docs/library_session_getfiletodirectory

            Console.WriteLine("Download complete...");

            // filepath of saved file.
            string newfilepath = targetdirectory + @"\" + Path.GetFileName(ftpfilepath);
            string decryptedfilepath = Directory.GetParent(newfilepath) + @"\" + FunctionTools.GetFileNameWithoutExtension(newfilepath);

            // run command line commands
            string targusdirectorycheck = @"C:\targus\tds\pgp";

            if (Directory.Exists(targusdirectorycheck))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string executedecryption = String.Format($@"C:\targus\tds\pgp\e1pgp.bat decrypt tdssys {newfilepath} {decryptedfilepath}");

                try
                {
                    
                    // define command
                    Process decryptcommand = new Process();
                    decryptcommand.StartInfo.FileName = "cmd.exe";
                    decryptcommand.StartInfo.Arguments = "/c " + executedecryption;
                    decryptcommand.StartInfo.UseShellExecute = false;
                    decryptcommand.StartInfo.CreateNoWindow = true;
                    decryptcommand.StartInfo.RedirectStandardOutput = true;
                    decryptcommand.StartInfo.RedirectStandardError = true;
                    //decryptcommand.StartInfo.RedirectStandardInput = true;

                    decryptcommand.Start();

                    //write output to console
                    while (!decryptcommand.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(decryptcommand.StandardOutput.ReadLine());
                    }

                    //write to console in one block
                    //string output = decryptcommand.StandardOutput.ReadToEnd();
                    //Console.WriteLine(output);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine(@"Directory Not Found: C:\targus\tds\pgp");
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
