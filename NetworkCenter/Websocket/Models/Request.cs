using System.Text.Json;

namespace NetworkCenter.Websocket.Models;

public class Request
{
    public readonly string Path;
    public readonly JsonElement Data;
}