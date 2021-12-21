using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WinSCP;
using System.IO;


namespace FileProcessor
{
    public class FTPLogins
    {

        public static Session LoginToOnboarding()
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

                return session;
            }
        }

        public static SessionOptions Onboarding()
        {
            SessionOptions sessionsettings = new SessionOptions();
            
            string username = FunctionTools.GetUser();
            string passcode = string.Empty;

            if (username != "dwhite")
            {
                passcode = FunctionTools.GetPasscode();
            }
            else
            {
                StreamReader passcodefile = new StreamReader(@"D:\Users\dwhite\Desktop\Notes\fileprocessorcredentials.txt");

                passcode = passcodefile.ReadLine().Trim();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Connecting to onboarding...");
            Console.ResetColor();

            // copied from winscp.
            sessionsettings.Protocol = Protocol.Sftp;
            sessionsettings.HostName = "onboarding.neustar.biz";
            sessionsettings.UserName = username;
            sessionsettings.Password = passcode;
            sessionsettings.SshHostKeyFingerprint = "ssh-rsa 1024 MKA9n3CYF8dY+j9P713bUoWelyJtFdv8gNpfn8pkzoc=";

            return sessionsettings;
        }

        public static SessionOptions E1Platform()
        {
            SessionOptions sessionsettings = new SessionOptions();

            sessionsettings.Protocol = Protocol.Sftp;
            sessionsettings.HostName = "download.targusinfo.com";
            sessionsettings.UserName = "e1platform";
            sessionsettings.Password = "Tu5wq$m4loPav";
            sessionsettings.SshHostKeyFingerprint = "ssh-rsa 1024 bP0rkp9/gF5HkewPta4i6HL6kItfx6b1cLJS7+K3HmA=";

            return sessionsettings;
        }

    }
}
