using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket.Controllers;

[Controller]
public class SampleController(ClientConnection client) : RequestBase(client)
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