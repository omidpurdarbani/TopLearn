using System;
using System.Collections.Generic;
using System.Text;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {

        bool IsExistUserName(string userName);

        bool IsExistEmail(string email);

        int AddUser(User user);

        User LoginUser(string Email, string Password);

        User ActiveAccount(string ActiveCode);

    }
}
