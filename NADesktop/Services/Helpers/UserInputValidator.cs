using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NADesktop.Services.Helpers;
internal static class UserInputValidator
{
    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6 ||
        !Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$"))
        {
            MessageBox.Show("Пароль должен состоять из латинских букв, длина пароля не меньше 6 символов с хотя бы одной цифрой и заглавной буквой.");
            return false;
        }
        return true;
    }

    public static bool ValidateLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login) || login.Length < 4)
        {
            MessageBox.Show("Логин должен быть не менее 4 символов.");
            return false;
        }
        return true;
    }

    public static bool ValidateEmail(string email)
    {
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            MessageBox.Show("Email некорректный");
            return false;
        }
    }
}
