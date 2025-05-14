using Calgon.Host.Controllers.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calgon.Host.Controllers.Users;

[Route("users")]
internal sealed class UsersController : ApplicationController
{
    [HttpPost]
    public UserCreatedModel CreateUser([FromBody] CreateUserModel model)
    {
        throw new NotImplementedException();
    }
}