using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.DataLayer.Entities.User
{
    public class Roles
    {
        public Roles()
        {

        }

        [Key]
        public int RoleId { get; set; }

        [Display(Name = "نام نقش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string RoleTitle { get; set; }

        #region Relations

        public virtual List<User> UserRoles { get; set; }

        public virtual List<RolePermissions> RolePermissions { get; set; }

        #endregion
    }

}
