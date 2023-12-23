
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Models.Response;
using NetworkAnalytics.Services.Clients;
using NetworkAnalytics.Services.DataAccess;
using NetworkAnalytics.Services.Entities;
using NetworkAnalytics.Services.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace NetworkAnalytics.Controllers;

[Route("vkAPI/")]
[ApiController]
[Authorize]
public class VKController(ContextDB ContextDB) : ControllerBase
{
    private readonly VKHelper VKDBHelper = new(ContextDB);

    [HttpPost("addattr")]
    public async Task<IActionResult> AdditionalVKAttributes([FromBody] LogginHelper authModel)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            UserManager? userSession = UserManager.GetUser(idUser);
            if (userSession == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));

            LogginHelper telegramUser = await VKDBHelper.AuthorizationAttributes(userSession, authModel);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(telegramUser.SomeObject),
                telegramUser,
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

    [HttpGet("allchats")]
    public async Task<IActionResult> GetAllChats()
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            UserManager? userSession = UserManager.GetUser(idUser);
            if (userSession == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));

            LogginHelper telegramUser = await VKDBHelper.GetDialogs(userSession);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(telegramUser.SomeObject),
                telegramUser,
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

    [HttpGet("hundredmessages/{idDialog}/{messageOffsetId}")]
    public async Task<IActionResult> GetHundredMessages(long idDialog, int messageOffsetId)
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            UserManager? userSession = UserManager.GetUser(idUser);
            if (userSession == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));

            LogginHelper telegramUser = await VKDBHelper.GetMessages(userSession, idDialog, messageOffsetId);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(telegramUser.SomeObject),
                telegramUser,
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

    [HttpDelete("signout")]
    public async Task<IActionResult> SignoutVK()
    {
        string token = "";
        try
        {
            int idUser = AuthOptions.GetUserIdFromToken(User);
            UserManager? userSession = UserManager.GetUser(idUser);
            if (userSession == null)
                return BadRequest(new ResponseModel(
                    null,
                    new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Login again"),
                    null)
                );
            token = new JwtSecurityTokenHandler().WriteToken(AuthOptions.NewToken(User.Claims));

            LogginHelper telegramUser = await VKDBHelper.Signout(userSession);
            return Ok(new ResponseModel(
                JsonSerializer.Serialize(telegramUser.SomeObject),
                telegramUser,
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
