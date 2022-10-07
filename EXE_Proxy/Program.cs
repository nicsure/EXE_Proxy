using System.Diagnostics;

const char q = '"';

string? exe = Environment.ProcessPath; // get the path+name of the exe
if (exe != null) Go(exe, args); // make sure we got it, if so continue with the Go() method

static void Go(string exe, string[] args)
{
    if (exe.ToLower().EndsWith(".exe")) exe = exe[..^4]; // remove .exe from end of executable
    string arg = string.Empty; // init the args rebuild string
    foreach (string a in args) // loop through all args
        arg += QuoteString(a) + " "; // add quotes if the arg has a space in it and append it to our rebuild string with a space to seperate each arg
    arg = arg.Trim(); // remove the extra space from the end
    string orig = exe + "_orig.exe"; // init the string for the original exe
    string[] log = { QuoteString(orig) + " " + arg + "\r\n\r\n" }; // construct the debug command line log quoting the exe if needed. Using an array to maybe add verbosity options in the future?
    using Process process = new(); // initialize a new process
    process.StartInfo.FileName = orig; // set the original exe to run
    process.StartInfo.Arguments = arg; // pass in the rebuilt argument string
    process.StartInfo.RedirectStandardOutput = true; // redirect console output streams to save in a log file
    process.StartInfo.RedirectStandardError = true;
    process.StartInfo.WorkingDirectory = Environment.CurrentDirectory; // set the same working dir as this app is working from
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // don't show a window
    try
    {
        process.Start(); // start the process
    }
    catch (Exception e) // catch any errors (yes I know, sue me)
    {
        lock(log)
            log[0] += "An error occured when trying to start the original exe \r\n\r\n" + e.ToString(); // add the error report to the log
    }
    if (!process.HasExited) // if the process is running
    {
        Log(process.StandardOutput, "OUTPUT: ", log); // start asynchronous tasks to capture console output
        Log(process.StandardError,  " ERROR: ", log);
    }
    try { process.WaitForExit(); } catch { } // wait for process to end (if it even started)
    WriteLogFile(exe + "_log.txt", log); // write out the log file
}

static void WriteLogFile(string fileName, string[] log)
{
    try
    {
        lock (log)
            File.WriteAllText(fileName, log[0]); // write text out to a file
    }
    catch { }
}

static async void Log(StreamReader stream, string prefix, string[] log)
{
    try
    {
        while (!stream.EndOfStream) // loop forever(ish)
        {
            string? line = await stream.ReadLineAsync(); // wait for the next line and release execution back to the Go() method
            if (line == null) break; // end the loop if there's an error
            lock (log)
                log[0] += prefix + line + "\r\n"; // add the prefix and line to the log buffer
        }
    }
    catch { }
}

static string QuoteString(string s) => s.IndexOf(" ") > -1 ? q + s + q : s; // adds quotes to a string if the string contains a space