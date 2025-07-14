namespace NetworkCenter.Websocket.Models;

public abstract class ControllerBase()
{
    protected ClientConnection Client { get; private set; }
    
    public void SetClient(ClientConnection client)
    {
        if (Client != null)
        {
            throw new InvalidOperationException("Client is already set");
        }

        Client = client;
    }
}