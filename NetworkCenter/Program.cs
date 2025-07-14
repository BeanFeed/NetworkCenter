using DAL.Context;
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
        
        if (!Directory.Exists(Path.Join(Environment.CurrentDirectory, "data")))
        {
            Directory.CreateDirectory(Path.Join(Environment.CurrentDirectory, "data"));
        }
        
        if (!File.Exists(Path.Join(Environment.CurrentDirectory, "data", "networkdata.db")))
        {
            File.Create(Path.Join(Environment.CurrentDirectory, "data", "networkdata.db")).Close();
        }
        
        SocketServerManager server = new SocketServerManager(Configuration);
        
        server.Services.AddDbContext<NetworkContext>();
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