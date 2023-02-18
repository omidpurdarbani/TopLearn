using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.User
{
    public class Permissions
    {
        public Permissions()
        {

        }

        [Key]
        public int PermissionId { get; set; }

        public string Name { get; set; }

        #region Relations

        public virtual List<RolePermissions> RolePermissions { get; set; }

        #endregion
    }
}
