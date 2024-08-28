using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;

namespace InjectedTimer
{
    public sealed class Pipe : IDisposable
    {
        public Pipe()
        {
            server = new NamedPipeServerStream("SpeedToolPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            toSend = new ConcurrentQueue<string>();

            server.WaitForConnection();
            pipeWriter = new StreamWriter(server);
            pipeReader = new PipeReader(server);
        }

        public void Dispose()
        {
            server.Dispose();
        }

        public void SendString(string str)
        {
            toSend.Enqueue(str);
        }

        public void Cycle()
        {
            if(!server.IsConnected)
                return;

            while(!toSend.IsEmpty)
            {
                SendNext();
            }
            pipeWriter.Flush();
        }

        private void SendNext()
        {
            if(toSend.IsEmpty)
                return;
            string res;
            while(!toSend.TryDequeue(out res));
            pipeWriter.WriteLine(res);
        }

        ConcurrentQueue<string> toSend;

        private StreamWriter pipeWriter;
        PipeReader pipeReader;
        private NamedPipeServerStream server;
    }
}
