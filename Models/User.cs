using System.Text.Json.Serialization;

namespace HangfireAPI.Models;

public class User
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}
