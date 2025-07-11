using Fleck;
using NetworkCenter.Websocket.Enums;

namespace NetworkCenter.Websocket.Models;

public class ClientConnection(IWebSocketConnection connection, ConnectionType type, string name, string key)
{
    public IWebSocketConnection Connection { get; } = connection;
    public ConnectionType Type { get; } = type;
    public string Name { get; } = name;
    public string Key { get; } = key;
}