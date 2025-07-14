using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket.Controllers;

[Controller]
[Authenticate]
public class SampleController : ControllerBase
{
    
    [Route("sample/echo")]
    public string Echo(string message)
    {
        return $"Echo: {message}";
    }

    [Route("sample/hello")]
    public string Hello()
    {
        return "Hello, World!";
    }

    [Route("sample/greet")]
    public string Greet()
    {
        return $"Hello, {Client.Name}!";
    }
}