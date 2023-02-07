using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TopLearn.Core.DTOs.User
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

    public class ChangePasswordViewModel
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string CurrentPassword { get; set; }

        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Compare("NewPassword", ErrorMessage = "کلمه های عبور مغایرت دارند")]
        public string ReNewPassword { get; set; }
    }
}
