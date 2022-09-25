using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.Core.DTOs
{
    public class InformationUserViewModel
    {

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime RegisterDate { get; set; }

        public int Wallet { get; set; }

    }

    public class SideBarUserPanelViewModel
    {
        public string UserName { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ImageName { get; set; }
    }

    public class EditProfileViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string UserName { get; set; }

        [Display(Name = "آواتار")]
        public IFormFile UserAvatar { get; set; }

        public string AvatarName { get; set; }

        public bool OldAvatar { get; set; }
    }
}
