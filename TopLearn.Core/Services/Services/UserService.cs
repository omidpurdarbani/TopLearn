using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;

namespace TopLearn.Core.Services.Services
{
    public class UserService : IUserService
    {

        private TopLearnContext _context;

        public UserService(TopLearnContext context)
        {
            _context = context;
        }
        
        public bool IsExistUserName(string userName)
        {
            return _context.users.Any(p=>p.UserName==userName);
        }

        public bool IsExistEmail(string email)
        {
            return _context.users.Any(p => p.Email == email);
        }

    }
}
