using System.Diagnostics;
using System.Reflection;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket;

public class RouteRegistry(SocketServerManager server)
{
    public readonly Dictionary<string, MethodInfo> Routes = new Dictionary<string, MethodInfo>();

    public void Map()
    {
        var controllers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(ControllerBase)) && x.GetCustomAttribute(typeof(ControllerAttribute)) != null).ToList();

        foreach (var controller in controllers)
        {
            string controllerName = controller.Name.Replace("Controller", "");
            
            var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttribute<RouteAttribute>(false) != null)
                .ToList();

            server.Services.AddTransient(controller);

            foreach (var method in methods)
            {
                
                string path;

                if (method.GetCustomAttribute<RouteAttribute>(false)!.Path == null)
                {
                    path = $"{controllerName}/{method.Name}";
                }
                else
                {
                    path = method.GetCustomAttribute<RouteAttribute>(false)!.Path!;
                }
                
                path = path.ToLower();
                
                if (!Routes.TryAdd(path, method))
                {
                    Debug.WriteLine($"Duplicate route found: {path} in {controller.Name}.{method.Name}");
                }
                else
                {
                    Debug.WriteLine($"Registered route: {path} -> {controller.Name}.{method.Name}");
                }
            }
        }
    }
}