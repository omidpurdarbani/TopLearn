using System.Collections.Generic;
using TopLearn.Core.DTOs.User;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {

        #region User account

        bool IsExistEmail(string email);

        int AddUser(User user);

        User LoginUser(string Email, string Password);

        User ActiveAccount(string ActiveCode);

        User GetUserByEmail(string Email);

        int GetUserIdByEmail(string Email);

        User GetUserByActiveCode(string ActiveCode);

        void UpdateUser(User user);

        #endregion

        #region User panel

        InformationUserViewModel GetUserInformation(string UserEmail);

        SideBarUserPanelViewModel GetSideBarUserPanelData(string UserEmail);

        EditProfileViewModel GetDataForEditUserProfile(string UserEmail);

        void EditProfile(string UserEmail, EditProfileViewModel profile);

        bool CompareCurrentPassword(string UserEmail, string CurrentPassword);

        void ChangeUserPassword(string UserEmail, string NewPassword);

        #endregion

        #region Wallet

        int UserWalletBalance(string UserEmail);

        List<WalletViewModel> GetUserWallet(string UserEmail);

        int ChargeWallet(string userEmail, int amount, string description, bool isPay = false);

        int AddWallet(Wallet wallet);

        Wallet GetWalletByWalletId(int WalletId);

        void UpdateWalletFactorUrl(int WalletId, string FactorUrl);

        void UpdateWallet(Wallet wallet);

        #endregion

        #region Admin Panel

        UsersForAdminViewModel GetUsers(int take = 10, int pageId = 1, string filterEmail = "", string filterUserName = "");

        #endregion

    }
}
