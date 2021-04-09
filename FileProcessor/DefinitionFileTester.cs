using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FluentFTP;

namespace FileProcessor
{
    public class DefinitionFileTester
    {
        public static void E1PlatformFileTester()
        {
            //select definition file
            string definitionfilepath = E1PlatformSelectDefinitionFile();

            //login to FTPClient
            FtpClient client = new FtpClient();
            FunctionTools.FTPE1platformLogin(client);

            //test target file and definition file. 
            string inputfile = DownloadAndTestDefinitionFile(client, definitionfilepath);

            client.Disconnect();

            //Generate test file
            GenerateTestFile(inputfile);
        }



        // tools
        public static string E1PlatformSelectDefinitionFile()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Select definition file to test against on the e1platform FTP");
            

            Console.WriteLine();
            Console.WriteLine("Select definition file:");
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

            Console.WriteLine("{0,5}", "exit");
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

        public static string DownloadAndTestDefinitionFile(FtpClient client, string definitionfilepath)
        {
            //download definition file.
            string tempdefinitionfile = "tempdefintionfile.definition"; //saved in debug folder.
            client.DownloadFile(tempdefinitionfile, definitionfilepath);

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
                    Console.WriteLine("File number of columns EQUAL definition columns.");

                    foreach (var column in headersplit)
                    {
                        int index = Array.IndexOf(headersplit, column);

                        if (column.ToUpper() != columnnamesupper[index].ToUpper()) //upper
                        {
                            Console.WriteLine("file column -{2}- no match: {0} - {1}", column, columnnames[index], index + 1);
                            matched = false;
                        }
                    }
                }
                else if (columnnamesupper.Count() <= headersplitupper.Count())
                {
                    int columndiff = headersplitupper.Count() - columnnamesupper.Count();

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
                }
                else
                {
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
                }

                if (matched == false) //same number of columns but different order or +/- column.
                {
                    Console.Write("Generating new .definition from file headers? (y/n): ");
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

                        using (StreamWriter newdefinitionfile = new StreamWriter(FunctionTools.GetDesktopDirectory() + "\\" + definitionfilepath.Split('/').Last()))
                        {
                            foreach (var value in fileinfo)
                            {
                                newdefinitionfile.WriteLine(value);
                            }

                            int columnnumber = 1;
                            foreach (var column in headersplit)
                            {
                                string linebuilder = "column" + columnnumber + ".fieldname = ";
                                newdefinitionfile.WriteLine(linebuilder + column);
                                columnnumber += 1;
                            }
                        }
                    }
                }

                if (morecolumns == true || lesscolumns == true)
                {
                    Console.Write("Generating list of columns from target file headers? (y/n): ");
                    string yesno = Console.ReadLine().ToUpper();

                    if (yesno.ToUpper() == "y".ToUpper())
                    {
                        using (StreamWriter newdefinitionfile = new StreamWriter(FunctionTools.GetDesktopDirectory() + "\\newcolumnlistfordefinitionfile.txt"))
                        {
                            newdefinitionfile.WriteLine("Target file column List");
                            newdefinitionfile.WriteLine();
                            newdefinitionfile.WriteLine();
                            newdefinitionfile.WriteLine();

                            foreach (var value in fileinfo)
                            {
                                newdefinitionfile.WriteLine(value);
                            }

                            int columnnumber = 1;
                            foreach (var column in headersplit)
                            {
                                string linebuilder = "column" + columnnumber + ".fieldname = ";
                                newdefinitionfile.WriteLine(linebuilder + column);
                                columnnumber += 1;
                            }
                        }
                    }
                }
            }

            return file;
        }

        // test files
        public static void GenerateTestFile(string inputfile)
        {
            Console.WriteLine("Generate Test File (y/n)?: ");
            string generatefileinput = Console.ReadLine().ToLower().Trim();

            if (generatefileinput == "y")
            {
                GetSubsetOfRecords(inputfile);
            }
        }

        public static void GetSubsetOfRecords(string filepath)  //get subset of file.
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
    }
}
