using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[PrimaryKey(nameof(Key))]
public class SocketKey
{
    public Guid Key { get; set; }
    public List<string> Scopes { get; set; } = new();
}