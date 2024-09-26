using DCI.Models.Entities;
using DCI.Models.ViewModel;


namespace DCI.Repositories.Interface
{
    public interface IUserRepository : IDisposable
    {
        Task<IList<User>> GetAllUsers();
        Task<User> GetUserById(int userid);
		Task<(int statuscode, string message)> Registration(UserViewModel model);
        Task<User> GetUserByEmail(string email);
     
        Task<LoginViewModel> Login(string username);
        Task<(int statuscode, string message)> Delete(int id);

        Task<bool> IsExistsUsername(string username);

		Task<(int statuscode, string message)> SaveExternalUser(ExternalUserModel model);
		Task<User> GetUserByUsername(string username);

        Task<(int statuscode, string message)> UpdateUser(UserViewModel model);
        Task<UserModel> GetUserRoleListById(int userid);
        Task<(bool isExists, string email)> GetExternalUser(ExternalUserModel model);
	}
}
