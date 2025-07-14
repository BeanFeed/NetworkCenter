namespace NetworkCenter.Websocket.Models;

public abstract class ControllerBase(ClientConnection client)
{
    protected ClientConnection Client => client;
}