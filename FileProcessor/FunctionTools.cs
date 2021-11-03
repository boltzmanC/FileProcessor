using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using WinSCP;

namespace FileProcessor
{
    public class FunctionTools
    {
        // list of generic methods to be used throughout the app.
        public static string GetAFile()
        {
            Console.Write("File:");

            string path = Console.ReadLine().Replace("\"", "");

            Console.WriteLine();

            return path;
        }

        public static string GetADirectory()
        {
            Console.Write("Enter directory: ");

            string path = Console.ReadLine().Replace("\"", "").Trim();

            Console.WriteLine();

            return path;
        }

        public static char GetDelimiter()
        {
            Console.Write("Enter Delimeter: ");
            var delimeter = Console.ReadLine();

            char splitchar;

            while (!char.TryParse(delimeter, out splitchar))
            {
                Console.Write("Invalid Enter Delimiter");
                delimeter = Console.ReadLine();
            }

            return splitchar;
        }

        public static char GetTXTQualifier()
        {
            Console.Write("Enter Txt Qualifier: ");
            var delimeter = Console.ReadLine();

            char splitchar;

            while (!char.TryParse(delimeter, out splitchar))
            {
                Console.Write("Entry Invalid");
                delimeter = Console.ReadLine();
            }

            Console.WriteLine();

            return splitchar;
        }

        public static int GetANumber()
        {
            Console.Write("Enter a number (Int32): ");
            string input = Console.ReadLine().Trim();

            int number = 0;
            if (int.TryParse(input, out number))
            {
                return number;
            }
            else
            {
                Console.WriteLine("Not a valid number. Try again...");
                return GetANumber();
            }
        }

        public static string GetUser()
        {
            Console.WriteLine();
            Console.Write("Enter username: ");
            string user = Console.ReadLine().Trim().ToLower();

            return user;
        }

        public static string GetPasscode()
        {
            Console.WriteLine();
            Console.WriteLine("Enter passcode: ");
            string passcode = Console.ReadLine().Trim();

            return passcode;
        }

        public static string[] SplitLineWithTxtQualifier(string expression, char delimiter, char qualifier, bool ignoreCase) //true -> sets everything to lower.
        {
            if (ignoreCase)
            {
                expression = expression.ToLower();
                delimiter = char.ToLower(delimiter);
                qualifier = char.ToLower(qualifier);
            }

            int len = expression.Length;
            char symbol;
            List<string> list = new List<string>();
            string newField = null;

            for (int begin = 0; begin < len; ++begin)
            {
                symbol = expression[begin];

                if (symbol == delimiter || symbol == '\n')
                {
                    list.Add(string.Empty);
                }
                else
                {
                    newField = null;
                    int end = begin;

                    for (end = begin; end < len; ++end)
                    {
                        symbol = expression[end];
                        if (symbol == qualifier)
                        {
                            // bypass the unsplitable block of text
                            bool foundClosingSymbol = false;
                            for (end = end + 1; end < len; ++end)
                            {
                                symbol = expression[end];
                                if (symbol == qualifier)
                                {
                                    foundClosingSymbol = true;
                                    break;
                                }
                            }

                            if (false == foundClosingSymbol)
                            {
                                throw new ArgumentException("expression contains an unclosed qualifier symbol");
                            }

                            continue;
                        }

                        if (symbol == delimiter || symbol == '\n')
                        {
                            newField = expression.Substring(begin, end - begin);
                            begin = end;
                            break;
                        }
                    }

                    if (newField == null)
                    {
                        newField = expression.Substring(begin);
                        begin = end;
                    }

                    list.Add(newField.Replace("\"", string.Empty)); //added to remove " for simplification.
                }
            }
            return list.ToArray();
        }

        public static string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        public static string GetFileNameWithoutExtension(string targetfile)
        {
            string filename = Path.GetFileNameWithoutExtension(targetfile);

            return filename;
        }

        public static string GetParentFolder(string targetfile)
        {
            return Directory.GetParent(targetfile).ToString();
        }

        public static void DecompressFile(string targetfile)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?view=net-5.0

            FileInfo targetfileinfo = new FileInfo(targetfile);

            using (FileStream targetfilestream = targetfileinfo.OpenRead())
            {
                string filename = targetfileinfo.FullName;
                string newfilename = filename.Remove(filename.Length - targetfileinfo.Extension.Length);

                using (FileStream decompressedfilestream = File.Create(newfilename))
                {
                    using (GZipStream decompressionstream = new GZipStream(targetfilestream, CompressionMode.Decompress))
                    {
                        decompressionstream.CopyTo(decompressedfilestream);
                        Console.WriteLine($"Decompressed: {targetfileinfo.Name}");
                    }
                }
            }
        }

        public static void CompressFile(string targetfile) 
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream?view=net-5.0

            FileInfo targetfileinfo = new FileInfo(targetfile);

            using (FileStream originalfilestream = targetfileinfo.OpenRead())
            {
                if ((File.GetAttributes(targetfileinfo.FullName) &
                   FileAttributes.Hidden) != FileAttributes.Hidden & targetfileinfo.Extension != ".gz")
                {
                    using (FileStream compressedfilestream = File.Create(targetfileinfo.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedfilestream, CompressionMode.Compress))
                        {
                            originalfilestream.CopyTo(compressionStream);
                        }
                    }
                    FileInfo info = new FileInfo(GetParentFolder(targetfileinfo.ToString()) + Path.DirectorySeparatorChar + targetfileinfo.Name + ".gz");
                    Console.WriteLine($"Compressed {targetfileinfo.Name} from {targetfileinfo.Length.ToString()} to {info.Length.ToString()} bytes.");
                }
            }
        }

        public static  Dictionary<string, string> GetListofDefinitionFiles (List<string> storeddefinitionfilepaths)
        {
            Dictionary<string, string> definitionfilelookup = new Dictionary<string, string>();

            foreach (var fileinfo in storeddefinitionfilepaths)
            {
                // hard coded values for simplicity, correct way to do this would be to test all input and verify
                string[] splitfileinfo = fileinfo.Split(',');

                if (!definitionfilelookup.ContainsKey(splitfileinfo[0]))
                {
                    definitionfilelookup.Add(splitfileinfo[0].Trim(), splitfileinfo[1].Trim()); // trim to handle typo spaces.
                }
            }

            return definitionfilelookup;
        }

        public static void LorealChangeFileNamestoReload(string targetdirectory)
        {
            //LOREAL_BIOL_20211012120215_HH.csv - file name

            // get extension
            // split on _
            // take first two values from split
            // append "reload"
            // reappend extension

            string reload = "_reload";
            int firstvalue = 1;
            int countofvalues = 2;

            string[] filepaths = Directory.GetFiles(targetdirectory); 

            foreach (string file in filepaths)
            {
                // take in new file name
                // break file name up. 
                
                string extension = Path.GetExtension(file);
                string filename = Path.GetFileNameWithoutExtension(file);

                string[] splitfilename = filename.Split('_');

                string newfilename = string.Join("_", splitfilename, firstvalue, countofvalues) + reload + extension;
                    //https://www.geeksforgeeks.org/c-sharp-join-method-set-1/

                File.Move(file, newfilename);
            }
        }


        //introduction
        public static void Introduction()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("File Processor for E1Monthly Files by Dylan White");
            Console.WriteLine();
            Console.ResetColor();
        }

        //console size.
        public static void ConsoleSize()
        {
            //setconsole size
            Console.SetWindowSize(150, 45);
            int bufferwidth = Console.BufferWidth;
            int bufferheight = 600;
            Console.SetBufferSize(bufferwidth, bufferheight);
        }

        // exit
        public static void ExitApp()
        {
            //Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
