using Fleck;
using NetworkCenter.Websocket.Enums;

namespace NetworkCenter.Websocket.Models;

public class ClientConnection(IWebSocketConnection connection, ConnectionType? type, string? name, string? key)
{
    public IWebSocketConnection Connection { get; } = connection;
    public ConnectionType? Type { get; private set; } = type;
    public string? Name { get; private set; } = name;
    public string? Key { get; private set; } = key;
    
    public string[] Scopes { get; set; } = [];
    
    public void Authenticate(ConnectionType type, string name, string key)
    {
        if (Type != null && Name != null && Key != null)
        {
            throw new InvalidOperationException("Client is already authenticated");
        }
        Type = type;
        Name = name;
        Key = key;
    }
}