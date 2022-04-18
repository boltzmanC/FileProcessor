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
    public class AdditionalTools
    {

        public static void MultiFileUniqueValueCheck()
        {
            // read through files and get dupes 
            // return dupes per file if any

            // using UNIQUE values per file retrun dupes across all files

            // output
            // unique ID | dupecount | files

            //start
            Console.WriteLine();
            //Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Find duplicate IDs across all files in a directory, ID column MUST be the same across files:");
            Console.WriteLine();


            //data storage
            // ID , List<string> = ( COUNT, files....)
            // list [0] is always count of ids 
            Dictionary<string, List<string>> alluniques = new Dictionary<string, List<string>>();

            // get data files
            string[] filelist = GetFiles();

            // delimiter
            char deli = FunctionTools.GetDelimiter();

            //txt q
            char txtq = FunctionTools.GetTXTQualifier();

            // get ID
            Console.Write("Enter column name for record ids: ");
            string recordid = Console.ReadLine().Trim().ToLower();


            //read files
            foreach (var f in filelist)
            {
                // read file
                string filename = FunctionTools.GetFileNameWithoutExtension(f); // get file name
                
                Dictionary<string, int> currentfilecounts = new Dictionary<string, int>(); // current file counts
                int countlines = 0;

                using (StreamReader targetfile = new StreamReader(f))
                {
                    string header = targetfile.ReadLine();

                    //get index of recordid
                    string[] headersplit = FunctionTools.SplitLineWithTxtQualifier(header, deli, txtq, true); // true to use toLOWER functionality
                    int recordidindex = Array.IndexOf(headersplit, recordid);
                    
                    string line = string.Empty;
                    while ((line = targetfile.ReadLine()) != null)
                    {
                        string[] linesplit = FunctionTools.SplitLineWithTxtQualifier(line, deli, txtq, false);
                        string id = linesplit[recordidindex];

                        if (currentfilecounts.ContainsKey(id))
                        {
                            currentfilecounts[id]++; //add count 
                        }
                        else
                        {
                            currentfilecounts.Add(id, 1); // new dict key
                        }

                        countlines++;
                    }
                }

                // check values against alluniques
                foreach (var id in currentfilecounts.Keys)
                {
                    if (alluniques.ContainsKey(id)) //update current values.
                    {
                        // get existing values from alluniques
                        List<string> currentvalues = alluniques[id]; 

                        //update id count
                        int currenttotal = Int32.Parse(currentvalues[0]);
                        currenttotal += currentfilecounts[id]; //use count of dupes from curent file if any.
                        currentvalues[0] = currenttotal.ToString(); // update current total

                        // add new filename
                        currentvalues.Add(filename);

                        // update values
                        alluniques[id] = currentvalues;
                    }
                    else
                    {
                        // build list of values
                        List<string> valuestoadd = new List<string> ();

                        valuestoadd.Add(currentfilecounts[id].ToString());
                        valuestoadd.Add(filename);

                        // update allunique dictionary
                        alluniques.Add(id, valuestoadd);
                    }
                }

                // status
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Read {filename}:");
                Console.WriteLine($"total records - {countlines}");
                Console.WriteLine($"unique IDs - { currentfilecounts.Keys.Count}");
                Console.ResetColor();

            }

            // output results
            string output = Directory.GetParent(filelist[0]).ToString();
            output += "\\MultiFileUniqueValueCheck_results.txt";

            using (StreamWriter outfile = new StreamWriter(output))
            {
                // write headers
                outfile.WriteLine("ID|Count|Files");
                
                foreach (var id in alluniques.Keys)
                {
                    // linebuilder
                    List<string> linebuilder = new List<string>();

                    linebuilder.Add(id);

                    // build rest of line.
                    List<string> savedvalues = alluniques[id];
                    
                    linebuilder.Add(savedvalues[0]); // add count.

                    savedvalues.RemoveAt(0); // remove count from savedvalues

                    linebuilder.Add(string.Join(deli.ToString(), savedvalues.ToArray())); // convert savedvalues to array from list and add , separated string as value to new list

                    outfile.WriteLine(string.Join("|", linebuilder.ToArray()));
                }
            }
        }

        private static string[] GetFiles()
        {
            
            string directory = FunctionTools.GetADirectory();

            if (Directory.Exists(@directory) == true)
            {
                // read all files in directory, then loop through
                string[] filepaths = Directory.GetFiles(@directory);

                return filepaths;
            }
            else
            {
                Console.ResetColor();
                Console.WriteLine($"Invalid entry: {directory}");

                MultiFileUniqueValueCheck();

                return null; // this is dumb
            }
            
            
        }

    }
}
