namespace NetworkCenter.Websocket.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute(params string[] scopes) : Attribute
{
    public string[] Scopes { get; private set; } = scopes;
}