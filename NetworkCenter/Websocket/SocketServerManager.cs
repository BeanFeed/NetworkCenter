using Fleck;
using Microsoft.Extensions.Configuration;

namespace NetworkCenter.Websocket;

public class SocketServerManager
{
    private readonly WebSocketServer _server;
    private readonly List<IWebSocketConnection> _connections = new();
    private readonly int _port;

    public SocketServerManager(IConfiguration configuration)
    {
        _port = int.TryParse(configuration["socket:port"], out var port) ? port : 8181;
        _server = new WebSocketServer($"ws://0.0.0.0:{_port}");
    }

    public void Start()
    {
        _server.Start(socket =>
        {
            socket.OnOpen = () => _connections.Add(socket);
            socket.OnClose = () => _connections.Remove(socket);
            socket.OnMessage = message =>
            {
                HandleRequest(socket, message);
            };
        });
    }

    public void Stop()
    {
        foreach (var conn in _connections)
        {
            conn.Close();
        }
        _connections.Clear();
        // Fleck does not have a direct Stop method, but you can dispose or recreate server as needed
    }

    public IReadOnlyList<IWebSocketConnection> Connections => _connections.AsReadOnly();
    
    private void HandleRequest(IWebSocketConnection socket, string message)
    {
        
    }
}