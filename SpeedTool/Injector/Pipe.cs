using System.IO.Pipes;
using SpeedTool.Platform.Debugging;

namespace SpeedTool.Injector;

public sealed class Pipe : IDisposable
{
    public Pipe()
    {
        client = new NamedPipeClientStream(".", "SpeedToolPipe", PipeDirection.InOut, PipeOptions.Asynchronous);
        connectCancellation = new();
        connectionTask = client.ConnectAsync(connectCancellation.Token);
        connectionTask.ContinueWith(x =>
        {
            DebugLog.SharedInstance.Write("Pipe connected");
            pipeReader = new PipeReader(client);
            pipeWriter = new StreamWriter(client);
        });
    }

    public bool IsOpened
    {
        get
        {
            if(pipeReader == null)
                return true;
            return pipeReader.IsOK;
        }
    }

    public void Dispose()
    {
        connectCancellation.Cancel();
        pipeReader?.Dispose();
        //pipeWriter?.Dispose();
        client.Dispose();
    }

    public void Cycle()
    {
        if(!client.IsConnected || pipeReader == null)
            return;
        while(pipeReader.HasData)
        {
            var line = pipeReader.ReadLine();
            if(line != null && line != "")
                ParseData(line);
        }
    }

    private void ParseData(string data)
    {
        if(data.StartsWith("debug_message "))
        {
            DebugLog.SharedInstance.Write($"InjectedTimer said: {data.Substring(14)}");
            return;
        }
        var tokens = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        switch(tokens[0])
        {
        default:
            break;
        }
    }
    Task? connectionTask;
    CancellationTokenSource connectCancellation;

    private PipeReader? pipeReader;
    private StreamWriter? pipeWriter;
    private NamedPipeClientStream client;
}
