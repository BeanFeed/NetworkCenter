using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket.Controllers;

[Controller]
public class AuthController(ClientConnection client) : ControllerBase(client)
{
    public bool Authenticate()
}