using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using DAL.Context;
using Fleck;
using Microsoft.EntityFrameworkCore;
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
                _ = HandleRequest(socket, message);
            };
        });
        
        _serviceProvider = Services.BuildServiceProvider();

        var context = _serviceProvider.GetService<NetworkContext>();
        context!.Database.Migrate();

        Task.Delay(-1).Wait();
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
    
    private async Task HandleRequest(IWebSocketConnection socket, string message)
    {
        JsonElement data = JsonSerializer.Deserialize<JsonElement>(message);
        if (data.TryGetProperty("path", out var id))
        {
            var path = id.GetString();
            bool routeExists = _routeRegistry.Routes.TryGetValue(path, out var route);
            if (!routeExists)
            {
                await socket.Send(JsonSerializer.Serialize(new { error = "Invalid request format" }));
            }
            Debug.Assert(route != null);
            if ((route.GetCustomAttribute<AuthenticateAttribute>(false) != null || route.DeclaringType.GetCustomAttribute<AuthenticateAttribute>() != null) && !ConnectionRegistry.Connections.Exists(x => x.Connection == socket && x is { Type: not null, Key: not null, Name: not null }))
            {
                await socket.Send(JsonSerializer.Serialize(new { status = "error", error = "You are not authenticated to the server." }));
                return;
            }

            try
            {
                var controllerType = route.DeclaringType!;
                

                //Get the controller instance from the service provider
                var controller = _serviceProvider.GetService(controllerType);
                
                if (controller == null)
                {
                    await socket.Send(JsonSerializer.Serialize(new { error = "Controller instance could not be created." }));
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
                    if (result is Task task)
                    {
                        await task.ConfigureAwait(false);
                        var taskType = task.GetType();
                        if (taskType.IsGenericType)
                        {
                            var returnValue = taskType.GetProperty("Result")!.GetValue(task);
                            await socket.Send(JsonSerializer.Serialize(new { status = "success", data = returnValue }, serializeOptions));
                        }
                        else
                        {
                            await socket.Send(JsonSerializer.Serialize(new { status = "success" }, serializeOptions));
                        }
                    }
                    else
                    {
                        if (result != null)
                        {
                            await socket.Send(JsonSerializer.Serialize(new { status = "success", data = result }, serializeOptions));
                        }
                    }
                }
                catch (Exception e)
                {
                    await socket.Send(JsonSerializer.Serialize(new { status = "error", error = e.InnerException.Message }));
                }
                
            }
            catch (Exception ex)
            {
                await socket.Send(JsonSerializer.Serialize(new { error = ex.Message }));
            }
        }
        else
        {
            await socket.Send(JsonSerializer.Serialize(new { error = "Invalid request format" }));
        }
    }
}