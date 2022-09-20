using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {

        bool IsExistEmail(string email);

        int AddUser(User user);

        User LoginUser(string Email, string Password);

        User ActiveAccount(string ActiveCode);

        User GetUserByEmail(string Email);

        User GetUserByActiveCode(string ActiveCode);

        void UpdateUser(User user);

    }
}
