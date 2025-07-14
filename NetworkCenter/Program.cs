using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkCenter.Websocket;
using NetworkCenter.Websocket.Services;

namespace NetworkCenter;

class Program
{
    //public static readonly string ConfigFolder = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location), "Config");
    private static readonly string ConfigPath = Path.Join(Environment.CurrentDirectory, "appsettings.json");
    public static IConfiguration Configuration = null!;
  
    static void Main(string[] args)
    {
        GetConfiguration();
        
        SocketServerManager server = new SocketServerManager(Configuration);

        server.Services.AddScoped<AuthService>();
        
        server.Start();
    }
    
    private static void GetConfiguration()
    {
        if (!File.Exists(ConfigPath))
        {
            File.WriteAllText(ConfigPath, "{}");
        }
        Console.WriteLine(ConfigPath);
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("socket.json", optional: true, reloadOnChange: true)
            .Build();
    }
}