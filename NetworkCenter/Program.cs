using Microsoft.Extensions.Configuration;

namespace NetworkCenter;

class Program
{
    //public static readonly string ConfigFolder = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location), "Config");
    private static readonly string ConfigPath = Path.Join(Environment.CurrentDirectory, "appsettings.json");
    public static IConfiguration Configuration = null!;
  
    static void Main(string[] args)
    {
        GetConfiguration();
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