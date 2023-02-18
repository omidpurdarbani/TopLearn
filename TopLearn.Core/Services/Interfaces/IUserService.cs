using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TopLearn.Core.DTOs.User;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {

        #region User account

        User GetUserByUserId(int userId);

        bool IsExistEmail(string email);

        bool IsUserExistByUserId(int userId);

        void AddUser(User user);

        User LoginUser(string email, string password);

        User ActiveAccount(string activeCode);

        User GetUserByEmail(string email);

        int GetUserIdByEmail(string email);

        User GetUserByActiveCode(string activeCode);

        void UpdateUser(User user);

        #endregion

        #region User panel

        InformationUserViewModel GetUserInformation(string userEmail);

        SideBarUserPanelViewModel GetSideBarUserPanelData(string userEmail);

        EditProfileViewModel GetDataForEditUserProfile(string userEmail);

        void EditProfile(string userEmail, EditProfileViewModel profile);

        bool CompareCurrentPassword(string userEmail, string currentPassword);

        void ChangeUserPassword(string userEmail, string newPassword);

        string SaveUserAvatar(IFormFile img, string avatar = null);

        #endregion

        #region Wallet

        int UserWalletBalance(string userEmail);

        List<WalletViewModel> GetUserWallet(string userEmail);

        int ChargeWallet(string userEmail, int amount, string description, bool isPay = false);

        int AddWallet(Wallet wallet);

        Wallet GetWalletByWalletId(int walletId);

        void UpdateWalletFactorUrl(int walletId, string factorUrl);

        void UpdateWallet(Wallet wallet);

        #endregion

        #region Admin Panel

        UsersForAdminViewModel GetUsers(int take = 10, int pageId = 1, string filterEmail = "", string filterUserName = "");

        UsersForAdminViewModel GetDeletedUsers(int take = 10, int pageId = 1, string filterEmail = "", string filterUserName = "");

        void AddUserFromAdmin(CreateUserViewModel user);

        EditUserViewModel GetUserInfoForEditUser(int userId);

        void EditUserFromAdmin(EditUserViewModel user, int userId);

        DeleteUserViewModel GetUserInfoForDeleteUser(int userId);

        void DeleteUserFromAdmin(int userId);

        RestoreUserViewModel GetUserInfoForRestoreUser(int userId);

        void RestoreUserFromAdmin(int userId);

        #endregion

    }
}
