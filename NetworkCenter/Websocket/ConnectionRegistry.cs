using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket;

public static class ConnectionRegistry
{
    public static List<ClientConnection> Connections { get; } = new List<ClientConnection>();
}