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
        static public string GetAFile()
        {
            Console.Write("File:");

            string path = Console.ReadLine().Replace("\"", "");

            Console.WriteLine();

            return path;
        }

        static public string GetAFileOrDirectory()
        {
            Console.Write("Enter directory or file path: ");

            string path = Console.ReadLine().Replace("\"", "").Trim();

            Console.WriteLine();

            return path;
        }

        static public char GetDelimiter()
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

        static public char GetTXTQualifier()
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

        static public string[] SplitLineWithTxtQualifier(string expression, char delimiter, char qualifier, bool ignoreCase) //true -> sets everything to lower.
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

        static public string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        static public string GetFileNameWithoutExtension(string targetfile)
        {
            string filename = Path.GetFileNameWithoutExtension(targetfile);

            return filename;
        }

        static public string GetParentFolder(string targetfile)
        {
            string parentfolder = Directory.GetParent(targetfile).ToString();
            return parentfolder;
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
