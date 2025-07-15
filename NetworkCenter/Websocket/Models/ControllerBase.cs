namespace NetworkCenter.Websocket.Models;

public abstract class ControllerBase()
{
    protected ClientConnection Client { get; private set; }
    protected ActionContext ActionContext { get; private set; }
    
    public void SetClient(ClientConnection client)
    {
        if (Client != null)
        {
            throw new InvalidOperationException("Client is already set");
        }

        Client = client;
    }
    
    public void SetActionContext(ActionContext context)
    {
        if (ActionContext != null)
        {
            throw new InvalidOperationException("ActionContext is already set");
        }

        ActionContext = context ?? throw new ArgumentNullException(nameof(context));
    }
}