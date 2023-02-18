using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
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

        public User GetUserByUserId(int userId)
        {
            return _context.Users.IgnoreQueryFilters().Single(u => u.UserId == userId);
        }

        public bool IsExistEmail(string email)
        {
            return _context.Users.Any(p => p.Email == email);
        }

        public bool IsUserExistByUserId(int userId)
        {
            return _context.Users.IgnoreQueryFilters().Any(u => u.UserId == userId);
        }

        public void AddUser(User user)
        {

            _context.Users.Add(user);
            _context.SaveChanges();

        }

        public User LoginUser(string email, string password)
        {
            var hashedPassword = PasswordHelper.EncodePasswordMd5(password);
            var fixedEmail = email.FixEmail();
            return _context.Users.SingleOrDefault(user => user.Password == hashedPassword && user.Email == fixedEmail);
        }

        public User ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
                return null;

            user.IsActive = true;
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.SaveChanges();

            return user;

        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(p => p.Email == email);
        }

        public int GetUserIdByEmail(string email)
        {
            return _context.Users.Where(p => p.Email == email).Select(u => u.UserId).SingleOrDefault();
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public void UpdateUser(User user)
        {
            user.ActiveCode = TextGenerator.GenerateUniqCode();
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public InformationUserViewModel GetUserInformation(string userEmail)
        {
            var user = GetUserByEmail(userEmail);

            InformationUserViewModel userInformation = new InformationUserViewModel();
            userInformation.Email = user.Email;
            userInformation.RegisterDate = user.RegisterDate;
            userInformation.UserName = user.UserName;
            userInformation.Wallet = UserWalletBalance(userEmail);

            return userInformation;
        }

        public SideBarUserPanelViewModel GetSideBarUserPanelData(string userEmail)
        {
            return _context.Users.Where(u => u.Email == userEmail).Select(p => new SideBarUserPanelViewModel()
            {
                UserName = p.UserName,
                RegisterDate = p.RegisterDate,
                ImageName = p.UserAvatar
            }).Single();
        }

        public EditProfileViewModel GetDataForEditUserProfile(string userEmail)
        {
            return _context.Users.Where(u => u.Email == userEmail).Select(u => new EditProfileViewModel()
            {
                AvatarName = u.UserAvatar,
                UserName = u.UserName
            }).Single();
        }

        public void EditProfile(string userEmail, EditProfileViewModel profile)
        {

            SaveUserAvatar(profile.UserAvatar, profile.AvatarName);

            var user = GetUserByEmail(userEmail);
            user.UserName = profile.UserName;
            user.UserAvatar = profile.AvatarName;

            UpdateUser(user);

        }

        public bool CompareCurrentPassword(string userEmail, string currentPassword)
        {
            string hashedCurrentPassword = PasswordHelper.EncodePasswordMd5(currentPassword);

            return _context.Users.Any(u => u.Email == userEmail && u.Password == hashedCurrentPassword);
        }

        public void ChangeUserPassword(string userEmail, string newPassword)
        {
            User user = GetUserByEmail(userEmail);

            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);

            UpdateUser(user);
        }

        public string SaveUserAvatar(IFormFile img, string avatar = null)
        {
            string imagePath;
            if (avatar != null && avatar != "Default.jpg")
            {
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", avatar);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            avatar = TextGenerator.GenerateUniqCode() + Path.GetExtension(img.FileName);

            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", avatar);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                img.CopyTo(stream);
            }

            return avatar;
        }

        public int UserWalletBalance(string userEmail)
        {

            int userId = GetUserIdByEmail(userEmail);

            var deposit = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 1 && w.IsPay)
                .Select(w => w.Amount)
                .ToList();

            var withDraw = _context.Wallets
                .Where(w => w.UserId == userId && w.TypeId == 2 && w.IsPay)
                .Select(w => w.Amount)
                .ToList();

            var remain = (deposit.Sum()) - (withDraw.Sum());

            return remain;

        }

        public List<WalletViewModel> GetUserWallet(string userEmail)
        {
            int useId = GetUserIdByEmail(userEmail);
            return _context.Wallets
                .Where(w => w.UserId == useId)
                .Select(p => new WalletViewModel()
                {
                    Amount = p.Amount,
                    DateTime = p.CreateDate,
                    Description = p.Description,
                    Type = p.TypeId,
                    FactorUrl = p.FactorUrl,
                    IsPay = p.IsPay
                })
                .ToList();
        }

        public int ChargeWallet(string userEmail, int amount, string description, bool isPay = false)
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
            return AddWallet(wallet);

        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();

            return wallet.WalletId;
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public void UpdateWalletFactorUrl(int walletId, string factorUrl)
        {
            Wallet wallet = GetWalletByWalletId(walletId);
            wallet.FactorUrl = factorUrl + walletId;
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public UsersForAdminViewModel GetUsers(int take = 10, int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = _context.Users;

            if (!string.IsNullOrWhiteSpace(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }

            if (!string.IsNullOrWhiteSpace(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            // Show item in page
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderByDescending(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public UsersForAdminViewModel GetDeletedUsers(int take = 10, int pageId = 1, string filterEmail = "",
            string filterUserName = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u => u.IsDelete);

            if (!string.IsNullOrWhiteSpace(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }

            if (!string.IsNullOrWhiteSpace(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }

            // Show item in page
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderByDescending(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public void AddUserFromAdmin(CreateUserViewModel user)
        {
            User addUser = new User()
            {
                Password = PasswordHelper.EncodePasswordMd5(user.Password),
                ActiveCode = TextGenerator.GenerateUniqCode(),
                Email = user.Email,
                IsActive = user.IsActive,
                UserName = user.UserName,
                RegisterDate = DateTime.Now,
                UserRole = user.SelectedRole
            };

            #region Save User Image

            if (user.UserAvatar == null)
            {
                addUser.UserAvatar = "Default.jpg";
            }
            else
            {
                addUser.UserAvatar = SaveUserAvatar(user.UserAvatar);
            }

            #endregion

            AddUser(addUser);
        }

        public EditUserViewModel GetUserInfoForEditUser(int userId)
        {
            return _context.Users.IgnoreQueryFilters().Where(u => u.UserId == userId).Select(u => new EditUserViewModel
            {
                CurrentAvatar = u.UserAvatar,
                IsActive = u.IsActive,
                UserName = u.UserName,
                Email = u.Email,
                SelectedRole = u.UserRole
            }).Single();
        }

        public void EditUserFromAdmin(EditUserViewModel user, int userId)
        {
            User editUser = GetUserByUserId(userId);
            //set edeitUser to user
            {
                if (user.Password != null)
                {
                    editUser.Password = PasswordHelper.EncodePasswordMd5(user.Password);
                }
                editUser.ActiveCode = TextGenerator.GenerateUniqCode();
                editUser.Email = user.Email;
                editUser.IsActive = user.IsActive;
                editUser.UserName = user.UserName;
                editUser.UserRole = user.SelectedRole;
            }

            #region Save User Image

            if (user.UserAvatar != null)
            {
                editUser.UserAvatar = SaveUserAvatar(user.UserAvatar, editUser.UserAvatar);
            }

            #endregion

            _context.Users.Update(editUser);
            _context.SaveChanges();
        }

        public DeleteUserViewModel GetUserInfoForDeleteUser(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new DeleteUserViewModel
                {
                    IsActive = u.IsActive,
                    Email = u.Email,
                    CurrentAvatar = u.UserAvatar,
                    SelectedRole = u.UserRole,
                    UserName = u.UserName
                }).Single();
        }

        public void DeleteUserFromAdmin(int userId)
        {
            var user = GetUserByUserId(userId);
            user.IsDelete = true;
            UpdateUser(user);
        }

        public RestoreUserViewModel GetUserInfoForRestoreUser(int userId)
        {
            return _context.Users.IgnoreQueryFilters().Where(u => u.UserId == userId)
                .Select(u => new RestoreUserViewModel
                {
                    IsActive = u.IsActive,
                    Email = u.Email,
                    CurrentAvatar = u.UserAvatar,
                    SelectedRole = u.UserRole,
                    UserName = u.UserName
                }).Single();
        }

        public void RestoreUserFromAdmin(int userId)
        {
            var user = GetUserByUserId(userId);
            user.IsDelete = false;
            UpdateUser(user);

        }
    }
}
