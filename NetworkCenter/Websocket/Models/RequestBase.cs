namespace NetworkCenter.Websocket.Models;

public abstract class RequestBase(ClientConnection client)
{
    protected ClientConnection Client => client;
}