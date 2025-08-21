using System.Text.Json.Serialization;

namespace NADesktop.Models.Domain;

public class User
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? SecondName { get; set; }
}
