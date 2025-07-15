using NetworkCenter.Websocket.Enums;
using NetworkCenter.Websocket.Models;

namespace NetworkCenter.Websocket.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthenticateAttribute : ActionFilterAttribute
{
    public override void BeforeExecute(ActionContext context)
    {
        if (context.Client.Name == null || context.Client.Key == null)
        {
            context.Status = ActionStatus.Error;
            context.Result = "You are not authenticated to the server.";
        }
    }

    public override void AfterExecute(ActionContext context) { }
}