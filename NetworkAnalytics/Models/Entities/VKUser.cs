
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetworkAnalytics.Interfaces;

namespace NetworkAnalytics.Models.Entities;

[Table("VkUser")]
public class VKUser : ISocialNetworkUser
{
    [Key, Required]
    public int? Id { get; set; }
    [Required]
    public int? IdUser { get; set; }
    public string? NickName { get; set; }
    public string? Login { get; set; }
    public byte[]? Icon { get; set; }
    public string? VkToken { get; set; }
    public string? Password { get; set; }
}
