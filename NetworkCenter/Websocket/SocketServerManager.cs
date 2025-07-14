using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Fleck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket;

public class SocketServerManager
{
    private readonly WebSocketServer _server;
    private readonly List<IWebSocketConnection> _connections = new();
    private readonly int _port;
    public readonly ServiceCollection Services = new ServiceCollection();
    private ServiceProvider _serviceProvider;
    private readonly RouteRegistry _routeRegistry;

    public SocketServerManager(IConfiguration configuration)
    {
        _port = int.TryParse(configuration["socket:port"], out var port) ? port : 8181;
        _server = new WebSocketServer($"ws://0.0.0.0:{_port}");
        _routeRegistry = new RouteRegistry(this);
    }

    public void Start()
    {
        _routeRegistry.Map();
        
        _server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _connections.Add(socket);
                ConnectionRegistry.Connections.Add(new ClientConnection(socket, null, null, null));
            };
            socket.OnClose = () =>
            {
                _connections.Remove(socket);
                ConnectionRegistry.Connections.Remove(ConnectionRegistry.Connections.First(x => x.Connection == socket));
            };
            socket.OnMessage = message =>
            {
                HandleRequest(socket, message);
            };
        });
        
        _serviceProvider = Services.BuildServiceProvider();
        Console.ReadKey();
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
        JsonElement data = JsonSerializer.Deserialize<JsonElement>(message);
        if (data.TryGetProperty("path", out var id))
        {
            var path = id.GetString();
            bool routeExists = _routeRegistry.Routes.TryGetValue(path, out var route);
            if (!routeExists)
            {
                socket.Send(JsonSerializer.Serialize(new { error = "Invalid request format" }));
            }
            Debug.Assert(route != null);
            if ((route.GetCustomAttribute<AuthenticateAttribute>(false) != null || route.DeclaringType.GetCustomAttribute<AuthenticateAttribute>() != null) && !ConnectionRegistry.Connections.Exists(x => x.Connection == socket && x is { Type: not null, Key: not null, Name: not null }))
            {
                socket.Send(JsonSerializer.Serialize(new { status = "error", error = "You are not authenticated to the server." }));
                return;
            }

            try
            {
                var controllerType = route.DeclaringType!;
                

                //Get the controller instance from the service provider
                var controller = _serviceProvider.GetService(controllerType);
                
                if (controller == null)
                {
                    socket.Send(JsonSerializer.Serialize(new { error = "Controller instance could not be created." }));
                    return;
                }
                
                // Invoke the SetClient method
                if (controller is ControllerBase controllerBase)
                {
                    var connection = ConnectionRegistry.Connections.FirstOrDefault(x => x.Connection == socket);
                    if (connection == null) connection = new ClientConnection(socket, null, null, null);
                    controllerBase.SetClient(connection);
                }
                

                var parameters = route.GetParameters();
                object?[] args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (data.TryGetProperty(parameters[i].Name, out var value))
                    {
                        args[i] = JsonSerializer.Deserialize(value.GetRawText(), parameters[i].ParameterType, new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    else
                    {
                        args[i] = null; // or handle missing parameters as needed
                    }
                }

                try
                {
                    var result = route.Invoke(controller, args);
                    var serializeOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    if (result != null)
                    {
                        socket.Send(JsonSerializer.Serialize(new { status = "success", data = result }, serializeOptions));
                    }
                }
                catch (Exception e)
                {
                    socket.Send(JsonSerializer.Serialize(new { status = "error", error = e.InnerException.Message }));
                }
                
            }
            catch (Exception ex)
            {
                socket.Send(JsonSerializer.Serialize(new { error = ex.Message }));
            }
        }
        else
        {
            socket.Send(JsonSerializer.Serialize(new { error = "Invalid request format" }));
        }
    }
}