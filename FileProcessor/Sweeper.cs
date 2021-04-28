using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WinSCP;
using System.Diagnostics;
using System.Reflection;
using System.Resources;


namespace FileProcessor
{
    public class Sweeper
    {
        private const char splitchar = '|';

        public static void AutoOnBoardingFTPSweeper()
        {
            // resource file location
            // documentation https://www.codeproject.com/Questions/5274073/Read-text-from-resources-Csharp
            string e1analyticsinternalfilelist = Properties.Resources.E1AnalyticsInternalFileList; 

            // e1analytics saved fileinfo dictionary.
            Dictionary<string, DateTime> e1analyticssavedfileinfo = new Dictionary<string, DateTime>();

            // e1analytics new file info
            Dictionary<string, DateTime> e1analyticsftpfileinfo = new Dictionary<string, DateTime>();

            //e1sas01
            string e1sas = @"\\e1sas01\O\Client Files";
            string ddrive = @"D:\";


            // read resouce file.
            using (StreamReader resoucereader = new StreamReader(e1analyticsinternalfilelist))
            {
                string line;

                while ((line = resoucereader.ReadLine()) != null)
                {
                    try
                    {
                        string[] storedfileinfo = line.Split(splitchar);

                        DateTime filedate = DateTime.Parse(storedfileinfo[1]);

                        e1analyticssavedfileinfo.Add(storedfileinfo[0], filedate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to parse: ");
                        Console.WriteLine(e);
                    }
                }
            }

            //login to onboarding
            using (Session session = new Session())
            {
                int newfilecount = 0;

                // Connect
                int attempts = 3;

                // list files in INTERNAL directory.
                string ftpdirectory = "/E1Analytics/internal";

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

                // write out target drictory to console.
                //WriteFileListToConsole(session, directory);


                // read onboarding file list
                try
                {
                    Console.WriteLine($"Looking at: {ftpdirectory}");
                    RemoteDirectoryInfo directoryinfo = session.ListDirectory(ftpdirectory);

                    int filecount = directoryinfo.Files.Count();

                    Console.WriteLine($"There are {filecount} files in the directory.");

                    foreach (RemoteFileInfo fileinfo in directoryinfo.Files)
                    {
                        string filename = fileinfo.FullName;
                        DateTime filedate = fileinfo.LastWriteTime;
                        
                        if (e1analyticssavedfileinfo.ContainsKey(filename))
                        {
                            // same file name new version case                            
                            int compareresult = DateTime.Compare(e1analyticssavedfileinfo[filename], filedate);

                            if (compareresult > 0) //https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=net-5.0#System_DateTime_Compare_System_DateTime_System_DateTime_
                            {
                                e1analyticsftpfileinfo.Add(filename, filedate);
                            }
                        }
                        else
                        {
                            // add FULL file name (includes path) and last write time to dictionary.
                            e1analyticsftpfileinfo.Add(filename, filedate);
                            newfilecount++;
                        }
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

                Console.WriteLine($"{newfilecount} - new files found in {ftpdirectory}");

                // get file paths that are to be downloaded.
                List<string> filestodownload = SelectFilesToDownload(e1analyticsftpfileinfo);

                // foreach file download and decrypt to the O drive. 
                foreach (var file in filestodownload)
                {
                    Decryption.SweeperFileDownloadAndDecryptConsole(session, ftpdirectory, ddrive);
                    
                    if(e1analyticsftpfileinfo.ContainsKey(file))
                    {
                        e1analyticssavedfileinfo.Add(file, e1analyticsftpfileinfo[file]);
                    }
                }







            }

            // write resouce file.
            using (StreamWriter resoucewriter = new StreamWriter(e1analyticsinternalfilelist))
            {
                //https://docs.microsoft.com/en-us/dotnet/api/system.resources.resourcewriter?view=net-5.0
                
                // use all files from ftp. to update and remove no longer listed file and add new ones. 

                
                //IResourceWriter writer = new IResourceWriter()
                
                //foreach (var filename in e1analyticssavedfileinfo)
                //{
                    
                //}
            }



            // download and decrypt option. 
            //  list out all files on console. enable user to input space delimited ints that are used to choose needed files. 



            // debugger
            //Console.ReadKey();
        }

        private static void WriteFileListToConsole(Session session, string directory) 
        {
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

        private static List<string> SelectFilesToDownload(Dictionary<string, DateTime> ftpfileinfo)
        {
            int filenumber = 1;
            List<string> filelist = new List<string>();
            List<string> filestodownload = new List<string>();

            Console.WriteLine("Select Files to download: ");

            foreach (var file in ftpfileinfo.Keys)
            {
                Console.WriteLine($"{filenumber}. {Path.GetFileName(file)}");
                filelist.Add(file);
                filenumber++;
            }

            // ALL
            Console.WriteLine("all. Download all files");

            Console.WriteLine();
            Console.Write("Select files with space delimited integers: ");
            string input = Console.ReadLine().ToLower();

            switch (input)
            {
                case "all":
                    return filelist;
                
                default:
                    // parse out the numbers and grab the specified file names.
                    foreach (var i in input.Split(' '))
                    {
                        try
                        {
                            filestodownload.Add(filelist[Int32.Parse(i) - 1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unable to parse entered values: ");
                            Console.WriteLine(e.ToString());
                        }

                    }
                    return filestodownload;
            }
        }



    }
}
