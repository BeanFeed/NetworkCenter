using System.Net.WebSockets;
using NetworkCenter.Websocket.Enums;

namespace NetworkCenter.Websocket.Models;

public class ActionContext(ControllerBase controller, ClientConnection client, object?[] arguments)
{
    public ActionStatus? Status { get; set; }
    public ControllerBase Controller { get; set; } = controller ?? throw new ArgumentNullException(nameof(controller));
    public object? Result { get; set; }
    public object?[] Arguments { get; set; } = arguments ?? throw new ArgumentNullException(nameof(arguments));
    public readonly ClientConnection Client = client ?? throw new ArgumentNullException(nameof(client));
}