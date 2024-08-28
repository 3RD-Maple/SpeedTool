using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Threading;


// This code is a carbon copy of PipeReader from SpeedTool project
//
// You masy ask "Grim, why did you not make a shared library?"
// And I would have an answer for you. Having it as a shared
// library would increase the difficulty of injecting this dll
// into anything, as I would need to register the shared library
// to be accessible from anywhere (somehow). So, for now, just CC
// this code from SpeedTool if needed.
namespace InjectedTimer
{
    public sealed class PipeReader : IDisposable
    {
        public PipeReader(PipeStream pipe)
        {
            sr = new StreamReader(pipe);
            workerThread = new Thread(Worker);
            workerThread.Start();
        }

        public bool IsOK
        {
            get
            {
                return !IsClosing;
            }
        }

        public void Dispose()
        {
            sr.Dispose();
            workerThread.Join();
        }

        public bool HasData
        {
            get
            {
                return !lines.IsEmpty;
            }
        }

        public string ReadLine()
        {
            string ret;
            while(!lines.TryDequeue(out ret));
            return ret;
        }

        private void Worker()
        {
            while(!IsClosing)
            {
                try
                {
                var line = sr.ReadLine();
                if(line != null && line != "")
                    lines.Enqueue(line);
                if(line == null)
                    IsClosing = true;
                }
                catch
                {
                    IsClosing = true;
                }
            }
        }

        bool IsClosing = false;
        Thread workerThread;
        ConcurrentQueue<string> lines = new ConcurrentQueue<string>();

        private StreamReader sr;
    }
}
