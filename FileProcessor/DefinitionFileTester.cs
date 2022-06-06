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
        private const string lorealstring = "Loreal";
        private const string allstring = "ALL";

        //testing loreal
        public static bool testingloreal = false;


        public static void E1PlatformFileTester()
        {
            //select definition file
            //string definitionfilepath = E1PlatformSelectDefinitionFile();
            List<string> definitionfilepaths = E1PlatformSelectDefinitionFilenew();

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

                foreach (var path in definitionfilepaths)
                {

                    //if all loreal files are selected...
                    if (testingloreal == true)
                    {
                        //test loreal files and genereate test files for all.
                        LorealDownloadAndTestDefinitionFile(session, path);
                        
                        //reset value.
                        //testingloreal = false;
                    }
                    else
                    {
                        //test target file and definition file. 
                        string inputfile = DownloadAndTestDefinitionFile(session, path);

                        //Generate test file
                        GenerateTestFile(inputfile);
                    }

                }
            }
        }

        // tools
        private static string E1PlatformSelectDefinitionFile() //not used
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select definition file to test against on the e1platform FTP: ");

            // todo. make new constant class that will store the client files. makes more usable and readable. easy to add new files. 
            //List<string> defaultvalues = typeof(BingoValuesDefault).GetAllPublicConstantValues<string>();

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

        private static List<string> E1PlatformSelectDefinitionFilenew()
        {
            // get name + path for all definition files.
            Dictionary<string, string> definitionfilelookup = FunctionTools.GetListofDefinitionFiles(typeof(DefinitionFileList).GetAllPublicConstantValues<string>());

            // definition file path main menu. returns single file path, except when LOREAL option is chosen.
            string definitionfilepath = DefinitionFileListMainMenu(definitionfilelookup);

            List<string> templist = new List<string>();

            if (definitionfilepath == lorealstring)
            {
                Dictionary<string, string> lorealdefinitionfilelookup = FunctionTools.GetListofDefinitionFiles(typeof(LorealDefinitionFileList).GetAllPublicConstantValues<string>());

                string lorealdefinitionfilepath = DefinitionFileListLorealMenu(lorealdefinitionfilelookup);

                if (lorealdefinitionfilepath == allstring)
                {
                    foreach (var kvp in lorealdefinitionfilelookup)
                    {
                        templist.Add(kvp.Value);
                    }

                    //we will be testing all loreal brand files
                    testingloreal = true;

                    return templist;
                }
                else
                {
                    templist.Add(lorealdefinitionfilepath);
                    return templist;
                }
            }
            else
            {
                templist.Add(definitionfilepath);
                return templist;
            }
        }

        private static string DefinitionFileListMainMenu(Dictionary<string, string> definitionfilelookup)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select definition file to test against on the e1platform FTP: ");

            int filecount = definitionfilelookup.Count();

            for (int x = 0; x < filecount; x++)
            {
                Console.WriteLine("{0,5}{1,-10}", x + ". ", definitionfilelookup.ElementAt(x).Key);
            }

            //Loreal menu option.
            Console.WriteLine("{0,5}{1,-10}", filecount + ". ", "Loreal Brand Files Menu"); //set to file count to maintain number sequence

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
            else if (parsed && (index == filecount)) // loreal process breakout
            {
                return lorealstring;
            }
            else
            {
                Console.WriteLine("Invalid number entered, try again...");
                //E1PlatformFileTester(); //restart.
                DefinitionFileListMainMenu(definitionfilelookup); // restart same menu.
            }


            return definitionfilepath;
        }

        private static string DefinitionFileListLorealMenu(Dictionary<string, string> definitionfilelookup)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select definition file to test against on the e1platform FTP: ");

            int filecount = definitionfilelookup.Count();

            for (int x = 0; x < filecount; x++)
            {
                Console.WriteLine("{0,5}{1,-10}", x + ". ", definitionfilelookup.ElementAt(x).Key);
            }

            //Loreal menu option.
            Console.WriteLine("{0,5}{1,-10}", filecount + ". ", allstring); //set to file count to maintain number sequence

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
            else if (parsed && (index == filecount))
            {
                return allstring;
            }
            else
            {
                Console.WriteLine("Invalid number entered, try again...");
                //E1PlatformFileTester(); //restart. main menu.
                DefinitionFileListLorealMenu(definitionfilelookup); //loreal menu
            }

            return definitionfilepath;
        }

        private static string DownloadAndTestDefinitionFile(Session session, string definitionfilepath)
        {
            //download definition file.
            string tempdefinitionfile = Directory.GetCurrentDirectory() + "\\tempdefinitionfile.definition"; //saved in debug folder.
            //File.Create(tempdefinitionfile);

            
            
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

            //delete temp definition file.
            if (File.Exists(@"tempdefinitionfile.definition"))
            {
                File.Delete(@"tempdefinitionfile.definition");
            }

            //show current definition file being tested.
            string definitionfilename = definitionfilepath.Split('/').Last();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine($"    {definitionfilename} - definition file selected");
            Console.WriteLine();
            Console.ResetColor();

            //Get file to test
            Console.WriteLine("File to Test:");
            string file = FunctionTools.GetAFile();
            char delimiter = FunctionTools.GetDelimiter();
            char txtq = FunctionTools.GetTXTQualifier();

            string newdefinitionfilepath = Directory.GetParent(file) + "\\" + "NEW_" + definitionfilepath.Split('/').Last();

            DefinitionFileTestResults(file, delimiter, txtq, fileinfo, columnnames, newdefinitionfilepath);

            return file;
        }

        private static void LorealDownloadAndTestDefinitionFile(Session session, string definitionfilepath)
        {
            // need to rewrite to do the loops within this function. 
            // E1PlatformSelectDefinitionFilenew() needs to not loop through definition files 
            // loop needs to be moved to this function.
            // need to change how ALL LOREAL files is selected



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

            //get loreal home
            Console.WriteLine();
            Console.Write("Enter parent Direcotry of Loreal Files to test: ");

            string lorealfilehome = FunctionTools.GetADirectory();
            char delimiter = FunctionTools.GetDelimiter();
            char txtq = FunctionTools.GetTXTQualifier();
            
            //rename loreal files.
            //FunctionTools.LorealChangeFileNamestoReload(lorealfilehome); //removed to allow multiple restarts. this step now needs to be done before testing.

            //delete temp definition file.
            if (File.Exists(@"tempdefintionfile.definition"))
            {
                File.Delete(@"tempdefintionfile.definition");
            }

            //show current definition file being tested.
            string definitionfilename = definitionfilepath.Split('/').Last();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine($"    {definitionfilename} - definition file selected");
            Console.WriteLine();
            Console.ResetColor();
            
            //Get file to test
            string[] filepaths = Directory.GetFiles(lorealfilehome);

            foreach (var f in filepaths)
            {
                // look at one loreal file because we are already looping through the definition file paths
                // @"/users/data/e1platform/loreal/nightly/LOREAL_ACCT_reload.definition";

                // test file path to match definition file name
                string defininitionnamenoexe = FunctionTools.GetFileNameWithoutExtension(definitionfilename);
                string filenamenoexe = FunctionTools.GetFileNameWithoutExtension(f);
                if (defininitionnamenoexe == filenamenoexe)
                {
                    string newdefinitionfilepath = Directory.GetParent(f) + "\\" + "NEW_" + definitionfilepath.Split('/').Last(); 

                    DefinitionFileTestResults(f, delimiter, txtq, fileinfo, columnnames, newdefinitionfilepath);

                    //Generate test file
                    GetSubsetOfRecords(f);
                }
            }
        }


        // test files
        private static void DefinitionFileTestResults(string file, char delimiter, char txtq, List<string> fileinfo, List<string> columnnames, string newdefinitionfilepath)
        {
            List<string> columnnamesupper = new List<string>();
            foreach (var c in columnnames)
            {
                columnnamesupper.Add(c.ToUpper());
            }

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
                    }

                }

                if (morecolumns == true || lesscolumns == true)
                {
                    Console.Write("Generate new .definition with new file header? (y/n): ");
                    string yesno = Console.ReadLine().ToUpper();

                    if (yesno.ToUpper() == "y".ToUpper())
                    {
                        WriteNewDefinitionFile(newdefinitionfilepath, fileinfo, headersplit);
                    }
                }
            }

        }

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
            string failheaderdiff = "false";
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
                writefile.WriteLine($"failIfHeaderDiff = {failheaderdiff}");
                writefile.WriteLine($"emailresultsto = {emailresultsto}");
                
                //columnfields
                for (int i = 1; i <= columnnames.Count(); i++)
                {
                    writefile.WriteLine($"column{i}.fieldname = {columnnames[i - 1]}");
                    writefile.WriteLine($"column{i}.header = {columnnames[i - 1]}");
                    // todo
                    // add string and number test
                    // add label test
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
