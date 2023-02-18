using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.User
{
    public class RolePermissions
    {
        public RolePermissions()
        {

        }

        [Key]
        public int RP_ID { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        #region Relations

        public virtual Roles Role { get; set; }

        public virtual Permissions Permission { get; set; }

        #endregion
    }
}
