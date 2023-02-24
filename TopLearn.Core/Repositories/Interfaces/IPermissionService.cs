using System.Collections.Generic;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        List<Roles> GetRoles();

        void AddPermissionToRole(int roleId, int permissionId);

        #endregion


    }
}
