using Microsoft.EntityFrameworkCore.Internal;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Services
{
    public class UserService : IUserService
    {

        private TopLearnContext _context;

        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool IsExistEmail(string email)
        {
            return _context.users.Any(p => p.Email == email);
        }

        public int AddUser(User user)
        {

            _context.users.Add(user);
            _context.SaveChanges();
            return user.UserId;

        }

        public User LoginUser(string Email, string Password)
        {
            var HashedPassword = PasswordHelper.EncodePasswordMd5(Password);
            var FixedEmail = Email.FixEmail();
            return _context.users.SingleOrDefault(user => user.Password == HashedPassword && user.Email == FixedEmail);
        }

        public User ActiveAccount(string ActiveCode)
        {
            var user = _context.users.SingleOrDefault(u => u.ActiveCode == ActiveCode);
            if (user == null || user.IsActive)
                return null;

            user.IsActive = true;
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.SaveChanges();

            return user;

        }

        public User GetUserByEmail(string Email)
        {
            return _context.users.SingleOrDefault(p => p.Email == Email);
        }

        public User GetUserByActiveCode(string ActiveCode)
        {
            return _context.users.SingleOrDefault(u => u.ActiveCode == ActiveCode);
        }

        public void UpdateUser(User user)
        {
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.users.Update(user);
            _context.SaveChanges();
        }

        public InformationUserViewModel GetUserInformation(string UserEmail)
        {
            var User = GetUserByEmail(UserEmail);

            InformationUserViewModel UserInformation = new InformationUserViewModel();
            UserInformation.Email = User.Email;
            UserInformation.RegisterDate = User.RegisterDate;
            UserInformation.UserName = User.UserName;
            UserInformation.Wallet = 0;

            return UserInformation;
        }

        public SideBarUserPanelViewModel GetSideBarUserPanelData(string UserEmail)
        {
            return _context.users.Where(u => u.Email == UserEmail).Select(p => new SideBarUserPanelViewModel()
            {
                UserName = p.UserName,
                RegisterDate = p.RegisterDate,
                ImageName = p.UserAvatar
            }).Single();
        }

        public EditProfileViewModel GetDataForEditUserProfile(string UserEmail)
        {
            return _context.users.Where(u => u.Email == UserEmail).Select(u => new EditProfileViewModel()
            {
                AvatarName = u.UserAvatar,
                UserName = u.UserName
            }).Single();
        }

        public void EditProfile(string useremail, EditProfileViewModel profile)
        {
            if (profile.UserAvatar != null)
            {
                string imagePath = "";
                if (profile.AvatarName != "Defult.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                profile.AvatarName = TextGenerator.GenerateUniqCode() + Path.GetExtension(profile.UserAvatar.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }
            }

            var user = GetUserByEmail(useremail);
            user.UserName = profile.UserName;
            user.UserAvatar = profile.AvatarName;

            UpdateUser(user);

        }
    }
}
