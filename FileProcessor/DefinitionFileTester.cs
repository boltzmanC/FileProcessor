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
    public class DefinitionFileTester
    {
        public static void E1PlatformFileTester()
        {
            //select definition file
            string definitionfilepath = E1PlatformSelectDefinitionFile();

            //login to FTPClient
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

                //test target file and definition file. 
                string inputfile = DownloadAndTestDefinitionFile(session, definitionfilepath);

                //Generate test file
                GenerateTestFile(inputfile);
            }
        }



        // tools
        private static string E1PlatformSelectDefinitionFile()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select definition file to test against on the e1platform FTP: ");
            

            //Console.WriteLine();
            //Console.WriteLine("Select definition file:");
            Console.WriteLine("{0,5}{1,-10}", "1. ", "CCNA");
            Console.WriteLine("{0,5}{1,-10}", "2. ", "Chickfila");
            Console.WriteLine("{0,5}{1,-10}", "3. ", "Discount Tire");
            Console.WriteLine("{0,5}{1,-10}", "4. ", "Discover Deposits");
            Console.WriteLine("{0,5}{1,-10}", "5. ", "Entercom ALLHH");
            Console.WriteLine("{0,5}{1,-10}", "6. ", "Entercom");
            Console.WriteLine("{0,5}{1,-10}", "7. ", "Progressive");
            Console.WriteLine("{0,5}{1,-10}", "8. ", "Santander");
            Console.WriteLine("{0,5}{1,-10}", "9. ", "Verizon");
            Console.WriteLine("{0,5}{1,-10}", "10. ", "Edelman - StatSocial");
            Console.WriteLine("{0,5}{1,-10}", "11. ", "michelin");

            Console.WriteLine("{0,5}{1,-10}", "exit: ", "exit");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("Definition File: ");
            string input = Console.ReadLine().ToUpper();
            
            string definitionfilepath = string.Empty;

            switch (input)
            {
                case "1":
                    definitionfilepath = @"/users/data/e1platform/CCNA/nightly/tccc_newcustomers_hh_reload.definition";
                    break;

                case "2":
                    definitionfilepath = @"/users/data/e1platform/chickfila/nightly/cfa_hh_reload.definition";
                    break;

                case "3":
                    definitionfilepath = @"/users/data/e1platform/discounttire/nightly/discounttire_hh_reload.definition";
                    break;

                case "4":
                    definitionfilepath = @"/users/data/e1platform/discover_deposits/nightly/discover_deposits_hh_fixed_reload.definition";
                    break;

                case "5":
                    definitionfilepath = @"/users/data/e1platform/entercom/nightly/entercom_allhh_reload.definition";
                    break;

                case "6":
                    definitionfilepath = @"/users/data/e1platform/entercom/nightly/entercom_hh_reload.definition";
                    break;

                case "7":
                    definitionfilepath = @"/users/data/e1platform/progressive/nightly/progressive_hh_reload.definition";
                    break;

                case "8":
                    definitionfilepath = @"/users/data/e1platform/santander/nightly/santander_hh_reload.definition";
                    break;

                case "9":
                    definitionfilepath = @"/users/data/e1platform/verizon/nightly/verizoncustomer_hh_reload.definition";
                    break;

                case "10":
                    definitionfilepath = @"/users/data/e1platform/edelman/nightly/EDELMAN_STATSOCIAL_HH_reload.definition";
                    break;

                case "11":
                    definitionfilepath = @"/users/data/e1platform/Michelin/nightly/michelin_crm_reload.definition";
                    break;

                default:
                    Console.WriteLine("Not valid input.");
                    DefinitionFileTester.E1PlatformSelectDefinitionFile(); //start over.
                    break;
            }

            return definitionfilepath;
        }

        private static string DownloadAndTestDefinitionFile(Session session, string definitionfilepath)
        {
            //download definition file.
            string tempdefinitionfile = "tempdefintionfile.definition"; //saved in debug folder.
            session.GetFiles(definitionfilepath, tempdefinitionfile);

            List<string> fileinfo = new List<string>();
            List<string> columnnames = new List<string>();

            using (StreamReader readdefinitionfile = new StreamReader(tempdefinitionfile))
            {
                string line = string.Empty;
                while ((line = readdefinitionfile.ReadLine()) != null)
                {
                    if (line.Contains(".fieldname"))
                    {
                        string[] linevalues = line.Split('=');
                        string columnname = linevalues.Last().Trim();
                        columnnames.Add(columnname);
                    }
                    else
                    {
                        fileinfo.Add(line);
                    }
                }
            }

            List<string> columnnamesupper = new List<string>();
            foreach (var c in columnnames)
            {
                columnnamesupper.Add(c.ToUpper());
            }

            //delete temp definition file.
            if (File.Exists(@"tempdefintionfile.definition"))
            {
                File.Delete(@"tempdefintionfile.definition");
            }

            //Get file to test
            Console.WriteLine("File to Test:");
            string file = FunctionTools.GetAFile();
            char delimiter = FunctionTools.GetDelimiter();
            char txtq = FunctionTools.GetTXTQualifier();

            string newdefinitionfilepath = Directory.GetParent(file) + "\\" + "NEW_" + definitionfilepath.Split('/').Last();

            using (StreamReader readfile = new StreamReader(file))
            {
                bool matched = true;
                bool morecolumns = false;
                bool lesscolumns = false;

                string header = readfile.ReadLine();
                string[] headersplit = FunctionTools.SplitLineWithTxtQualifier(header, delimiter, txtq, false);

                List<string> headersplitupper = new List<string>();
                foreach (var c in headersplit)
                {
                    headersplitupper.Add(c.ToUpper());
                }

                if (columnnamesupper.Count() == headersplitupper.Count())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("File number of columns EQUAL definition columns.");
                    Console.ResetColor();
                    Console.WriteLine();

                    foreach (var column in headersplit)
                    {
                        int index = Array.IndexOf(headersplit, column);

                        if (column.ToUpper() != columnnamesupper[index].ToUpper()) //upper
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("BUT column names do not match definition file:");
                            Console.WriteLine($"file column -{index + 1}- no match: {column} -> {columnnames[index]}");
                            matched = false;

                            Console.ResetColor();
                            Console.WriteLine();
                        }
                    }
                }
                else if (columnnamesupper.Count() <= headersplitupper.Count())
                {
                    int columndiff = headersplitupper.Count() - columnnamesupper.Count();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("File number of columns GREATER THAN definition columns.");
                    Console.WriteLine("{0} - extra columns in new file.", columndiff);
                    

                    int columncount = 1;
                    foreach (var column in headersplit)
                    {
                        if (!columnnamesupper.Contains(column.ToUpper())) //upper
                        {
                            Console.WriteLine("Column {1}, not in definition file: {0}", column, columncount);
                        }
                        columncount += 1;
                    }

                    morecolumns = true;
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("File number of columns LESS THAN definition columns.");

                    int columncount = 1;
                    foreach (var column in columnnamesupper)
                    {
                        if (!headersplitupper.Contains(column))
                        {
                            Console.WriteLine("Column {1}, not in target file: {0}", column, columncount);
                        }
                        columncount += 1;
                    }

                    lesscolumns = true;
                    Console.ResetColor();
                    Console.WriteLine();
                }

                if (matched == false) //same number of columns but different order or +/- column.
                {
                    Console.Write("Generate new .definition from file headers? (y/n): ");
                    string yesno = Console.ReadLine().ToUpper();

                    if (yesno.ToUpper() == "y".ToUpper())
                    {
                        Console.Write("Keep definition file column names? (y/n): ");
                        string keepdefinitioncolumn = Console.ReadLine().ToUpper();

                        if (keepdefinitioncolumn == "y".ToUpper())
                        {
                            for (int i = 0; i < headersplit.Count(); i++)
                            {
                                if (headersplit[i].ToUpper() != columnnames[i].ToUpper())
                                {
                                    headersplit[i] = columnnames[i].ToUpper();
                                }
                            }
                        }

                        WriteNewDefinitionFile(newdefinitionfilepath, fileinfo, headersplit);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"New Definition file created: {newdefinitionfilepath}");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    
                }

                if (morecolumns == true || lesscolumns == true)
                {
                    Console.Write("Generate new .definition with new file header? (y/n): ");
                    string yesno = Console.ReadLine().ToUpper();

                    if (yesno.ToUpper() == "y".ToUpper())
                    {
                        WriteNewDefinitionFile(newdefinitionfilepath, fileinfo, headersplit);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"New Definition file created: {newdefinitionfilepath}");
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                }
            }

            return file;
        }

        // test files
        private static void GenerateTestFile(string inputfile)
        {
            Console.Write("Generate Test File (y/n)?: ");
            string generatefileinput = Console.ReadLine().ToLower().Trim();

            if (generatefileinput == "y")
            {
                GetSubsetOfRecords(inputfile);
            }
        }

        private static void GetSubsetOfRecords(string filepath)  //get subset of file.
        {
            //default # of records to read. in the future can make this based on file length. 
            int numbertoread = 10000;
            
            string filename = FunctionTools.GetFileNameWithoutExtension(filepath);
            string outfile = Directory.GetParent(filepath) + "\\" + filename + "_subset" + "_" + numbertoread + ".txt";

            //fix int for reading file.
            numbertoread -= 1;

            using (StreamWriter writeto = new StreamWriter(outfile))
            {
                using (StreamReader readfile = new StreamReader(filepath))
                {
                    string header = readfile.ReadLine();
                    writeto.WriteLine(header);

                    int countlines = 0;
                    string line;
                    while ((line = readfile.ReadLine()) != null && countlines <= numbertoread)
                    {
                        writeto.WriteLine(line);

                        countlines++;
                    }

                    if (countlines < numbertoread)
                    {
                        Console.WriteLine("Lines requested - {0}. Lines read - {1}", numbertoread, countlines);
                    }
                }
            }

            //Write Output to console
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Output file created: {outfile}");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void GetSubsetOfRecordsStandAlone()
        {
            //get file
            string filepath = FunctionTools.GetAFile();
            int numbertoread = FunctionTools.GetANumber();
            
            
            //default # of records to read. in the future can make this based on file length. 
            //int numbertoread = 10000;

            string filename = FunctionTools.GetFileNameWithoutExtension(filepath);
            string outfile = Directory.GetParent(filepath) + "\\" + filename + "_subset" + "_" + numbertoread + ".txt";

            //fix int for reading file.
            numbertoread -= 1;

            using (StreamWriter writeto = new StreamWriter(outfile))
            {
                using (StreamReader readfile = new StreamReader(filepath))
                {
                    string header = readfile.ReadLine();
                    writeto.WriteLine(header);

                    int countlines = 0;
                    string line;
                    while ((line = readfile.ReadLine()) != null && countlines <= numbertoread)
                    {
                        writeto.WriteLine(line);

                        countlines++;
                    }

                    if (countlines < numbertoread)
                    {
                        Console.WriteLine("Lines requested - {0}. Lines read - {1}", numbertoread, countlines);
                    }
                }
            }

            //Write Output to console
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Output file created: {outfile}");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void GenerateNewDefinitionFile()
        {
            // get user fileinfo
            string file = FunctionTools.GetAFile();
            char delimiter = FunctionTools.GetDelimiter();
            char txtq = FunctionTools.GetTXTQualifier();

            //save directory
            string filesavepath = Directory.GetParent(file).ToString();
            //DirectoryInfo filesavepath = Directory.GetParent(file);

            // save to file location or desktop?
            Console.Write("Save .definition to same location as source file? (y/n): ");
            string savetodesktop = Console.ReadLine().Trim().ToLower();
            

            if (savetodesktop == "n")
            {
                filesavepath = FunctionTools.GetDesktopDirectory().ToString();
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Path for new definition file: {filesavepath}");
                Console.ResetColor();
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Path for new definition file: {filesavepath}");
                Console.ResetColor();
                Console.WriteLine();
            }

            // ask user to set table name
            Console.Write("Enter table name to be used in e1x: ");
            string tablename = Console.ReadLine();

            //additional settings... wont change for now. 
            string definitionfilefirstline = "#FTP Automation Definition";
            string operation = "reload";
            string skipfirstrow = "true";
            string purgedefinition = "false";
            string operationmonitorhours = "17";
            string emailresultsto = "dylan.white@team.neustar,e1@support.neustar";


            // column list for definition file
            List<string> columnnames = new List<string>();
            
            // read file header
            using (StreamReader readfile = new StreamReader(file))
            {
                string readfirstline = readfile.ReadLine();

                string[] splitline = FunctionTools.SplitLineWithTxtQualifier(readfirstline, delimiter, txtq, false);

                foreach (var column in splitline)
                {
                    columnnames.Add(column.ToUpper().Trim());
                }
            }

            // create definition file based on template.
            string newdefinitionfilename = $"{tablename}_reload.definition";
            string definitionfile = Path.Combine(filesavepath, newdefinitionfilename);

            using (StreamWriter writefile = new StreamWriter(definitionfile))
            {
                writefile.WriteLine(definitionfilefirstline);
                writefile.WriteLine($"tablename = {tablename}");
                writefile.WriteLine($"operation = {operation}");
                writefile.WriteLine($"skipfirstrow = {skipfirstrow}");
                writefile.WriteLine($"delimiter = {delimiter}");
                writefile.WriteLine($"textqualifier = {txtq}");
                writefile.WriteLine($"purgedefinition = {purgedefinition}");
                writefile.WriteLine($"operationmonitorhours = {operationmonitorhours}");
                writefile.WriteLine($"emailresultsto = {emailresultsto}");
                
                //columnfields
                for (int i = 1; i <= columnnames.Count(); i++)
                {
                    writefile.WriteLine($"column{i}.fieldname = {columnnames[i - 1]}");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Definition file has been created: {definitionfile}");
                Console.ResetColor();
                Console.WriteLine();
            }

            //preview and change definition file.
            Console.Write("Preview definition file? (y/n): ");
            string preview = Console.ReadLine().ToLower().Trim();

            if (preview == "y")
            {
                using (StreamReader readdefinitionfile = new StreamReader(definitionfile))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;

                    string line = string.Empty;
                    while ((line = readdefinitionfile.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                Console.ResetColor();
                Console.WriteLine();
            }

            Console.Write("Do any changes need to be made? (y/n): ");
            string changes = Console.ReadLine().ToLower().Trim();

            if (changes == "y")
            {
                //startover
                Console.WriteLine("OK we will start again...");
                GenerateNewDefinitionFile();
            }

        }

        private static void WriteNewDefinitionFile(string definitionfilepath, List<string> fileinformation, string[] fileheader)
        {
            using (StreamWriter newdefinitionfile = new StreamWriter(definitionfilepath))
            {
                foreach (var value in fileinformation)
                {
                    newdefinitionfile.WriteLine(value);
                }

                int columnnumber = 1;
                foreach (var column in fileheader)
                {
                    string linebuilder = "column" + columnnumber + ".fieldname = ";
                    newdefinitionfile.WriteLine(linebuilder + column);
                    columnnumber += 1;
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"New Definition file created: {definitionfilepath}");
            Console.ResetColor();
            Console.WriteLine();

            //using (StreamWriter newdefinitionfile = new StreamWriter(newdefinitionfilepath))
            //{
            //    foreach (var value in fileinfo)
            //    {
            //        newdefinitionfile.WriteLine(value);
            //    }

            //    int columnnumber = 1;
            //    foreach (var column in headersplit)
            //    {
            //        string linebuilder = "column" + columnnumber + ".fieldname = ";
            //        newdefinitionfile.WriteLine(linebuilder + column);
            //        columnnumber += 1;
            //    }
            //}
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine($"New Definition file created: {newdefinitionfilepath}");
            //Console.ResetColor();
            //Console.WriteLine();
        }
    }
}
