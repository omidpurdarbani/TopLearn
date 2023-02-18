using System.Collections.Generic;
using System.Linq;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Services
{
    public class PermissionService : IPermissionService
    {
        private TopLearnContext _context;

        public PermissionService(TopLearnContext context)
        {
            _context = context;
        }

        public List<Roles> GetRoles()
        {
            var roles = _context.Roles.ToList();
            return roles;
        }

        public void AddPermissionToRole(int roleId, int permissionId)
        {
            //ToDo
            throw new System.NotImplementedException();
        }
    }
}
