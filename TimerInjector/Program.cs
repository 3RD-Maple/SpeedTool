using System;
using System.IO;

using EasyHook;

class Program
{
    static int Main(string[] args)
    {
        if(args.Length != 1)
            return 1;
        int targetPID = 0;
        if(!int.TryParse(args[0], out targetPID))
            return 1;

        if (targetPID <= 0)
            return 1;

        // Get the full path to the assembly we want to inject into the target process
        string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "InjectedTimer.dll");

        try
        {
            // inject into existing process
            RemoteHooking.Inject(
                targetPID,          // ID of process to inject into
                injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                ""                  // the parameters to pass into injected library
            );
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There was an error while injecting into target:");
            Console.ResetColor();
            Console.WriteLine(e.ToString());
        }

        return 0;
    }
}
