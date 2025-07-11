namespace NetworkCenter.Websocket.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    public string? Path { get; set; }

    public ControllerAttribute(string? path = null)
    {
        Path = path;
    }
}