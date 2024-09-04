namespace SpeedTool.Platform.EventsArgs;

public sealed class BeforeClosingEventArgs : EventArgs
{
    public BeforeClosingEventArgs() { }

    public bool ShouldClose { get; set; } = true;
}