using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;

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

    }
}
