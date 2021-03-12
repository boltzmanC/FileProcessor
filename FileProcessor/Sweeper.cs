using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FluentFTP;

namespace FileProcessor
{
    public class Sweeper
    {

        public static void AutoOnBoardingFTPSweeper()
        {
            //login to onboarding 
            //Sweep for new files
            string directory = "/E1Analytics/internal";

            if (Directory.Exists(directory))
            {
                string[] filelist = Directory.GetFiles(directory);



            }
            else
            {
                Console.WriteLine($"Directory {directory} not found.");
                FunctionTools.ExitApp();
            }
            
            // if new files are found -> copy to O drive.



        }

        public static void ManualFTPSweeper()
        {

        }

    }
}
