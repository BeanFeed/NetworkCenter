using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket.Attributes;

public abstract class ActionFilterAttribute : Attribute
{
    public abstract void BeforeExecute(ActionContext context);
    
    public abstract void AfterExecute(ActionContext context);
}