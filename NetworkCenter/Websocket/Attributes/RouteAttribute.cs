namespace NetworkCenter.Websocket.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public string? Path { get; }

    public RouteAttribute(string? path = null)
    {
        Path = path;
    }
}