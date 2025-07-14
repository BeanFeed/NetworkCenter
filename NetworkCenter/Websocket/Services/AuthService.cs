using NetworkCenter.Websocket.Enums;
using NetworkCenter.Websocket.Models;
using NetworkCenter.Websocket.Models.Auth;

namespace NetworkCenter.Websocket.Services;

public class AuthService
{
    public async Task<bool> Authenticate(AuthModel auth, ClientConnection client)
    {
        if (string.IsNullOrEmpty(auth.Name) || string.IsNullOrEmpty(auth.ConnectionType) || string.IsNullOrEmpty(auth.Key))
        {
            return false;
        }
        if(!Enum.TryParse<ConnectionType>(auth.ConnectionType, true, out var connectionType)) throw new Exception("Invalid connection type");
        if (client is { Name: not null, Key: not null, Type: not null })
        {
            return false;
        }
        client.Authenticate(connectionType, auth.Name, auth.Key);
        
        ConnectionRegistry.Connections.Add(client);

        return true;
    }
}