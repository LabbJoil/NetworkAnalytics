using NetworkAnalytics.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("TelegramUser")]
public class TelegramUser : ISocialNetworkUser
{
    [Key, Required]
    public int? Id { get; set; }
    [Required]
    public int? IdUser { get; set; }
    public string? NickName { get; set; }
    public string? Login { get; set; }
    public byte[]? Icon { get; set; }
    public string? Password { get; set; }
    public string? NameSession { get; set; }

    public void UpdateModel(TelegramUser newTUser)
    {
        NickName = newTUser.NickName;
        Icon = newTUser.Icon;
        Login = newTUser.Login;
    }
}
