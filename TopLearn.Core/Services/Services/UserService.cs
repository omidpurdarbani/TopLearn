using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

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

        public int GetUserIdByEmail(string Email)
        {
            return _context.users.SingleOrDefault(p => p.Email == Email).UserId;
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
            UserInformation.Wallet = UserWalletBalance(UserEmail);

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

        public bool CompareCurrentPassword(string UserEmail, string CurrentPassword)
        {
            string HashedCurrentPassword = PasswordHelper.EncodePasswordMd5(CurrentPassword);

            return _context.users.Any(u => u.Email == UserEmail && u.Password == HashedCurrentPassword);
        }

        public void ChangeUserPassword(string UserEmail, string NewPassword)
        {
            User user = GetUserByEmail(UserEmail);

            user.Password = PasswordHelper.EncodePasswordMd5(NewPassword);

            UpdateUser(user);
        }

        public int UserWalletBalance(string UserEmail)
        {

            int userId = GetUserIdByEmail(UserEmail);

            var Deposit = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay)
                .Select(w => w.Amount)
                .ToList();

            var WithDraw = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 2 && w.IsPay)
                .Select(w => w.Amount)
                .ToList();

            var Remain = (Deposit.Sum()) - (WithDraw.Sum());

            return Remain;

        }

        public List<WalletViewModel> GetUserWallet(string UserEmail)
        {
            int useId = GetUserIdByEmail(UserEmail);
            return _context.Wallets
                .Where(w => w.IsPay && w.UserId == useId)
                .Select(p => new WalletViewModel()
                {
                    Amount = p.Amount,
                    DateTime = p.CreateDate,
                    Description = p.Description,
                    Type = p.TypeId
                })
                .ToList();
        }

        public void ChargeWallet(string userEmail, int amount, string description, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByEmail(userEmail)
            };
            AddWallet(wallet);
        }

        public void AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }
    }
}
