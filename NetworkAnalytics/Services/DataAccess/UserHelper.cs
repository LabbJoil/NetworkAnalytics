using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.Request;
using NetworkAnalytics.Services.Clients;
using NetworkAnalytics.Services.Entities;
using NetworkAnalytics.Services.System;
namespace NetworkAnalytics.Services.DataAccess;

public class UserHelper(ContextDB contextDB)
{
    private readonly ContextDB Database = contextDB;

    public User GetUserInfo(int idUser)
    {
        User user = Database.Users.Where(user => user.Id == idUser).FirstOrDefault()
            ?? throw new Exception($"User with id ({idUser}) not found");
        user.Password = "";
        return user;
    }

    public User AuthorizeUsers(string login, string password)
    {
        User? user = Database.Users.FirstOrDefault(
            nextUser => nextUser.Login == login &&
            nextUser.Password == UserConverter.HashPassword(password ?? "")
            ) ?? throw new Exception("You don't have account");
        UserManager.AddUser(user);
        user.Password = "";
        return user;
    }

    public async Task<User> SetUser(UserRequestModel userRequestModel)
    {
        if (Database.Users.FirstOrDefault(user => user.Login == userRequestModel.Login) != null)
            throw new Exception($"User with Login ({userRequestModel.Login}) already exists");
        User userModel = new(userRequestModel);
        Database.Users.Add(userModel);
        await Database.SaveChangesAsync();
        userModel.Password = "";
        UserManager.AddUser(userModel);
        return userModel;
    }

    public async Task<User> UpdateUser(int idUser, UserRequestModel userModel)
    {
        User updateUser = Database.Users.Where(user => user.Id == idUser).FirstOrDefault()
            ?? throw new Exception($"User with id ({idUser}) not found");
        updateUser.CreateUserModel(userModel);
        await Database.SaveChangesAsync();
        updateUser.Password = "";
        return updateUser;
    }

    public async Task<bool> UpdatePassword(int idUser, PasswordRequestModel passwordModel)
    {
        User updateUser = Database.Users.Where(user => user.Id == idUser).FirstOrDefault()
            ?? throw new Exception($"User with id ({idUser}) not found");
        if (updateUser.Password == UserConverter.HashPassword(passwordModel.OldPassword ?? ""))
        {
            updateUser.SetPassword(passwordModel.NewPassword);
            await Database.SaveChangesAsync();
            return true;
        }
        else return false;
    }

    public List<Report> GetReportsUser(int idUser)
    {
        List<Report> reports = [.. Database.Reports.Where(report => report.IdUser == idUser)];
        return reports;
    }

    public AnalyticReport GetReportById(int idUser, int idReport)
    {
        AnalyticReport analyticReport = new()
        {
            MainInfo = Database.Reports.Where(report => report.IdUser == idUser && report.Id == idReport).FirstOrDefault() ?? throw new Exception("The report is missing"),
            AverageThem = Database.Thems.Where(them => them.IdReport == idReport).FirstOrDefault() ?? new(),
            AverageTonality = Database.Tonalitys.Where(tonality => tonality.IdReport == idReport).FirstOrDefault() ?? new(),
            AggregatePartsSpeech = Database.PartsSpeechs.Where(tonality => tonality.IdReport == idReport).FirstOrDefault() ?? new(),
            AggregateCommonWords = [.. Database.CommonWords.Where(cw => cw.IdReport == idReport)]
        };
        return analyticReport;
    }

    public async Task<bool> DeleteReport(int idUser, int IdReport)
    {
        var deleteReport = Database.Reports.Where(report => report.Id == IdReport && report.IdUser == idUser).FirstOrDefault();
        if (deleteReport != null)
        {
            Database.Reports.Remove(deleteReport);
            await Database.SaveChangesAsync();
            return true;
        }
        else
            return false;
    }

    public async Task DeleteUser(int idUser)
    {
        User deleteUser = Database.Users.FirstOrDefault(user => user.Id == idUser) ?? throw new Exception($"Пользователь с id ({idUser}) не найден");
        Database.Users.Remove(deleteUser);
        await Database.SaveChangesAsync();
    }
}
