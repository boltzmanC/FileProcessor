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
        private const char splitchar = '|';
        //private const string newlinesplit = "\r\n";


        //todo
        // change so that files are downloaded then dycrpted. no need to decrypt between downloads. this causes connection to sftp site to drop 

        public static void OnBoardingFTPSweeper()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Executing Onboarding File Sweep...");
            Console.WriteLine();
            Console.ResetColor();
            
            // e1analytics saved fileinfo dictionary.
            Dictionary<string, DateTime> e1analyticssavedfileinfo = new Dictionary<string, DateTime>();

            // e1analytics ftp file info
            Dictionary<string, DateTime> e1analyticsftpfileinfo = new Dictionary<string, DateTime>();

            // new files to download
            Dictionary<string, DateTime> newfilestodownload = new Dictionary<string, DateTime>();

            // Save Location
            string savelocation = SaveLocation();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine($"Save Location set to: {savelocation}");
            Console.WriteLine();
            Console.ResetColor();

            // new files found
            int newfilecount = 0;

            // list files in INTERNAL directory.
            string ftpdirectory = "/E1Analytics/internal";

            // read resouce file.
            string resourcefile = Directory.GetCurrentDirectory() + @"\E1AnalyticsInternalFileList.txt";
            bool readresourcefile = false;

            if (File.Exists(resourcefile))
            {
                e1analyticssavedfileinfo = ResourceFileReader(resourcefile);
                readresourcefile = true;
            }
            else
            {
                Console.WriteLine($"No resource file found, creating {resourcefile}");
                File.Create(resourcefile);
            }

            // login to onbaording
            using (Session session = new Session())
            {
                // Connect
                int attempts = 3;
                do
                {
                    try
                    {
                        session.Open(FTPLogins.Onboarding());
                        
                        //additional settings.
                        //session.AddRawConfiguration("Compression", "1"); // causes error to be thrown.
                        session.SessionLogPath = Directory.GetCurrentDirectory() + @"\sessionlog.txt";
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to connect - {e}");
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

                Console.WriteLine("Connected.");

                // read onboarding file list
                try
                {
                    Console.WriteLine($"Looking at: {ftpdirectory}");
                    RemoteDirectoryInfo directoryinfo = session.ListDirectory(ftpdirectory);

                    int filecount = 0;

                    foreach (RemoteFileInfo fileinfo in directoryinfo.Files)
                    {
                        if (fileinfo.IsDirectory == false)
                        {
                            // store all file info
                            e1analyticsftpfileinfo.Add(fileinfo.FullName, fileinfo.LastWriteTime);

                            if (e1analyticssavedfileinfo.ContainsKey(fileinfo.FullName) && readresourcefile == true)
                            {
                                // same file name new version case                            
                                int compareresult = DateTime.Compare(e1analyticssavedfileinfo[fileinfo.FullName], fileinfo.LastWriteTime);

                                if (compareresult > 0) //https://docs.microsoft.com/en-us/dotnet/api/system.datetime.compare?view=net-5.0#System_DateTime_Compare_System_DateTime_System_DateTime_
                                {
                                    newfilestodownload.Add(fileinfo.FullName, fileinfo.LastWriteTime);
                                }
                            }
                            else if (!e1analyticssavedfileinfo.ContainsKey(fileinfo.FullName)) // only add if not already in the save file.
                            {
                                // add FULL file name (includes path) and last write time to dictionary.
                                newfilestodownload.Add(fileinfo.FullName, fileinfo.LastWriteTime);
                                newfilecount++;
                            }
                            filecount++;
                        }
                    }
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"There are {filecount} file(s) in the {ftpdirectory} directory.");
                    Console.WriteLine($"{newfilecount} - new files found.");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                catch (WinSCP.SessionException error)
                {
                    Console.WriteLine($"SessionExceptionError: {error}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                if (newfilestodownload.Count() > 0)
                {
                    // get file paths that are to be downloaded.
                    List<string> filestodownload = SelectFilesToDownload(newfilestodownload);

                    if (filestodownload.Count == 0) // if the list is empty then start over. 
                    {
                        Processor.ProcessorStartMenu();
                    }

                    // foreach file download and decrypt to the O drive. 
                    foreach (var filepath in filestodownload)
                    {
                        // foreach full file name, download and decrypt to target directory.
                        Decryption.SweeperFileDownloadAndDecryptConsole(session, filepath, @savelocation);
                    }

                    // update resouce file.
                    ResourceFileWriter(e1analyticsftpfileinfo, resourcefile);

                    Console.WriteLine("Download(s) completed and resource file updated with latest file list...");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("No new files to download.");
                    Console.ResetColor();
                    Processor.ProcessorStartMenu();
                }
            }
        }


        // additional methods
        public static void OnboardingFTPWriteFileListToConsole() // for testing.
        {
            using (Session session = new Session())
            {
                string directory = "/E1Analytics/internal";

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

                try
                {
                    Console.WriteLine($"Looking at: {directory}");
                    RemoteDirectoryInfo directoryinfo = session.ListDirectory(directory);

                    int filecount = directoryinfo.Files.Count();

                    Console.WriteLine($"There are {filecount} files in the directory.");

                    foreach (RemoteFileInfo fileInfo in directoryinfo.Files)
                    {
                        if (fileInfo.IsDirectory == false)
                        {
                            //Console.WriteLine($"{fileInfo.Name} with size {fileInfo.Length}, permissions {fileInfo.FilePermissions} and last modification at {fileInfo.LastWriteTime}");
                            Console.WriteLine($"{fileInfo.Name}|{fileInfo.LastWriteTime}");
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
            }

            Console.ReadKey();
        }

        private static string SaveLocation()
        {
            string directorylookup = @"\\e1sas01\O\OnboardingSweepResults";
            string savelocation = @"D:\Data";

            Console.Write("Save to e1sas01 (y/n)?: ");
            string input = Console.ReadLine().Trim().ToLower();

            if (input == "y")
            {
                if (!Directory.Exists(directorylookup))
                {
                    Directory.CreateDirectory(directorylookup);
                    savelocation = directorylookup;
                    return savelocation;
                }
                else
                {
                    savelocation = directorylookup;
                    return savelocation;
                }
            }
            else
            {
                return savelocation;
            }

        }

        private static List<string> SelectFilesToDownload(Dictionary<string, DateTime> newfilestodownload)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            int filenumber = 1;
            List<string> filelist = new List<string>();
            List<string> filestodownload = new List<string>();

            Console.WriteLine("Files to download: ");

            foreach (var file in newfilestodownload.Keys)
            {
                Console.WriteLine($"{filenumber}. {Path.GetFileName(file)}");
                filelist.Add(file);
                filenumber++;
            }

            // ALL
            Console.WriteLine("all. Download all files");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Select files with space delimited integers: ");
            string input = Console.ReadLine().ToLower();

            switch (input)
            {
                case "all":
                    return filelist;

                case "cancel": //return emtpty list and return to top level menu
                    List<string> emptylist = new List<string>();
                    return emptylist;

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

        private static Dictionary<string, DateTime> ResourceFileReader(string resourcefilepath)
        {
            Dictionary<string, DateTime> storedfilelistresource = new Dictionary<string, DateTime>();

            using (StreamReader reader = new StreamReader(resourcefilepath))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    string[] linesplit = line.Split(splitchar);

                    try
                    {
                        DateTime filedate = DateTime.Parse(linesplit[1]);

                        storedfilelistresource.Add(linesplit[0], filedate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to parse: {linesplit}");
                        Console.WriteLine(e);
                    }
                }
            }
            // return dict of filename | lastwritetime
            return storedfilelistresource;
        }

        private static void ResourceFileWriter(Dictionary<string, DateTime> datainput, string resourcefilepath) // todo add notification of file first time this process is run and its save location.
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Updating filelist resouce file: {resourcefilepath}");
            using (StreamWriter writer = new StreamWriter(resourcefilepath))
            {
                foreach(var fileinfo in datainput.Keys)
                {
                    writer.WriteLine(string.Format($"{fileinfo}{splitchar}{datainput[fileinfo].ToString()}"));
                }
            }

            Console.WriteLine("Update complted...");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
