using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WinSCP;
using System.Diagnostics;

namespace FileProcessor
{
    public class Sweeper
    {

        public static void AutoOnBoardingFTPSweeper()
        {
            //login to onboarding
            using (Session session = new Session())
            {
                // Connect
                int attempts = 3;
                do
                {
                    try
                    {
                        Console.WriteLine("Connecting to onboarding...");
                        session.Open(FTPLogins.Onboarding());
                    }
                    catch (Exception e)
                    {
                        Console.Write($"Failed to connect - {e}");
                        if (attempts == 0)
                        {
                            // give up
                            Console.WriteLine("I give up...");
                            throw;
                        }
                    }
                    attempts--;
                }
                while (!session.Opened);

                Console.WriteLine("Connected.");

                // list files in INTERNAL directory.
                string directory = "/E1Analytics/internal";
                try
                {
                    Console.WriteLine($"Looking at: {directory}");
                    RemoteDirectoryInfo directoryinfo = session.ListDirectory(directory);

                    int filecount = directoryinfo.Files.Count();

                    Console.WriteLine($"There are {filecount} files in the directory.");

                    foreach (RemoteFileInfo fileInfo in directoryinfo.Files)
                    {
                        //Console.WriteLine($"{fileInfo.Name} with size {fileInfo.Length}, permissions {fileInfo.FilePermissions} and last modification at {fileInfo.LastWriteTime}");
                        Console.WriteLine($"{fileInfo.Name} - {fileInfo.LastWriteTime}");
                    }
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

            Console.ReadKey();
        }

        public static void ManualFTPSweeper()
        {

        }

    }
}
