using NetworkAnalytics.Models.Request;
using NetworkAnalytics.Services.System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("User")]
public class User
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public string? Login { get; set; }
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? SecondName { get; set; }

    public User(UserRequestModel userModel)
    {
        CreateUserModel(userModel);
        SetPassword(userModel.Password);
    }

    public User() { }

    public void CreateUserModel(UserRequestModel userModel)
    {
        if (!string.IsNullOrEmpty(userModel.Login)) Login = userModel.Login;
        if (!string.IsNullOrEmpty(userModel.Email)) Email = userModel.Email;
        if (!string.IsNullOrEmpty(userModel.Name)) Name = userModel.Name;
        if (!string.IsNullOrEmpty(userModel.SecondName)) SecondName = userModel.SecondName;
    }

    public void SetPassword(string? password)
    {
        if (!string.IsNullOrEmpty(password)) Password = UserConverter.HashPassword(password);
    }
}
