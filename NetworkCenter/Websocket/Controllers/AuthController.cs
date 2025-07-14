using NetworkCenter.Websocket.Attributes;
using NetworkCenter.Websocket.Models;
using NetworkCenter.Websocket.Models.Auth;
using NetworkCenter.Websocket.Services;

namespace NetworkCenter.Websocket.Controllers;

[Controller]
public class AuthController(AuthService authService) : ControllerBase
{
    [Route]
    public async Task<bool> Authenticate(AuthModel auth)
    {
        return await authService.Authenticate(auth, Client);
    }

    [Route]
    public bool Test()
    {
        return true;
    }
}