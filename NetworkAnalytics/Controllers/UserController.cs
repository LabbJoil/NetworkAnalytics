
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Models.Request;
using NetworkAnalytics.Models.Response;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Services.Background;
using NetworkAnalytics.Services.Clients;
using NetworkAnalytics.Services.DataAccess;
using NetworkAnalytics.Services.Entities;
using NetworkAnalytics.Services.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace NetworkAnalytics.Controllers;

[Route("user/")]
[ApiController]
public class UserController(ContextDB ContextDB) : ControllerBase
{
    private readonly UserHelper UserDBHelper = new(ContextDB);

    [HttpGet("userinfo")]
    [Authorize]
    public IActionResult GetNewTokenController()
    {
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null) throw new Exception("Login again");
            var jwt = AuthOptions.NewToken(User.Claims);
            User user = UserDBHelper.GetUserInfo(idUser);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(user),
                new(TypeMessage.Authorize, DangerLevel.Oke, "Successful"),
                new JwtSecurityTokenHandler().WriteToken(jwt)));
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, ex.Message),
                null)
                );
        }
    }

    [HttpGet("login/{login}/{password}")]
    public IActionResult LogInUserController(string login, string password)
    {
        try
        {
            User authorizeUser = UserDBHelper.AuthorizeUsers(login, password);
            var jwt = AuthOptions.NewToken(
                new List<Claim>
                {
                    new(ClaimTypes.Sid, authorizeUser.Id.ToString()),
                    new(ClaimTypes.Name, authorizeUser.Login!)
                }
             );

            return Ok(new ResponseModel(
                JsonSerializer.Serialize(authorizeUser),
                new LogginHelper(TypeMessage.Authorize, DangerLevel.Oke, "Good authorize"),
                new JwtSecurityTokenHandler().WriteToken(jwt))
                );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, ex.Message),
                null)
                );
        }
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUpUserController([FromBody] UserRequestModel userModel)
    {
        try
        {
            User authorizeUser = await UserDBHelper.SetUser(userModel);
            var jwt = AuthOptions.NewToken(
                new List<Claim>
                {   
                    new(ClaimTypes.Sid, authorizeUser.Id.ToString()),
                    new(ClaimTypes.Name, authorizeUser.Login!)
                }
             );
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(authorizeUser),
                new LogginHelper(TypeMessage.Authorize, DangerLevel.Oke, "Good Sing up"),
                new JwtSecurityTokenHandler().WriteToken(jwt))
                );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, ex.Message),
                null)
                );
        }
    }

    [HttpPut("updateUser")]
    [Authorize]
    public async Task<IActionResult> UpdateUserController([FromBody] UserRequestModel userModel)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null) 
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            User user = await UserDBHelper.UpdateUser(idUser, userModel);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(user),
                new LogginHelper(TypeMessage.Ordinary, DangerLevel.Oke, "You have changed your profile"),
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpPut("updatePassword")]
    [Authorize]
    public async Task<IActionResult> UpdatePasswordController([FromBody] PasswordRequestModel userModel)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            bool isUpdate = await UserDBHelper.UpdatePassword(idUser, userModel);
            LogginHelper updateLH;
            if (isUpdate) updateLH = new(TypeMessage.Ordinary, DangerLevel.Oke, "You update the password");
            else updateLH = new(TypeMessage.Problem, DangerLevel.Wanted, "You didnt`t update the password");
            return Ok(new ResponseModel(
                null,
                updateLH,
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpPost("doAnalytics")]
    [Authorize]
    public IActionResult DoAnalyticsController([FromBody] DialogModel dialogModel)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            BackgroundAnalyticsProcessor.AddDialog(idUser, dialogModel);
            return Ok(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Ordinary, DangerLevel.Oke, "Message analytics added to the queue"),
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpGet("reports")]
    [Authorize]
    public IActionResult GetReportsController()
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            List <Report> reports = UserDBHelper.GetReportsUser(idUser);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(reports),
                new LogginHelper(TypeMessage.Ordinary, DangerLevel.Oke, "List reports"),
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpGet("reportId/{idReport}")]
    [Authorize]
    public IActionResult GetReportByIdController(int idReport)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            AnalyticReport report = UserDBHelper.GetReportById(idUser, idReport);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(report),
                new LogginHelper(TypeMessage.Ordinary, DangerLevel.Oke, "Report"),
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpDelete("deleteReport/{idReport}")]
    [Authorize]
    public async Task<IActionResult> DeleteReportByIdController(int idReport)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            bool isDelete = await UserDBHelper.DeleteReport(idUser, idReport);
            LogginHelper deleteLH;
            if (isDelete) deleteLH = new(TypeMessage.Ordinary, DangerLevel.Oke, "You delete the report");
            else deleteLH = new(TypeMessage.Problem, DangerLevel.Wanted, "You didnt`t delete the report");
            return Ok(new ResponseModel(
                null,
               deleteLH,
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }

    [HttpDelete("deleteUser")]
    [Authorize]
    public async Task<IActionResult> DeleteUserController()
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            if (UserManager.GetUser(idUser) == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));
            await UserDBHelper.DeleteUser(idUser);
            return Ok(new ResponseModel(
                null,
               new(TypeMessage.Ordinary, DangerLevel.Oke, "You delete the report"),
                token)
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseModel(
                null,
                new LogginHelper(TypeMessage.Problem, DangerLevel.Wanted, ex.Message),
                token)
                );
        }
    }
}
