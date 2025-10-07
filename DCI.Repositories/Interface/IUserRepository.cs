using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System.Threading.Tasks;


namespace DCI.Repositories.Interface
{
    public interface IUserRepository : IDisposable
    {
        //Task<IList<User>> GetAllUsers();
        Task<IList<UserModel>> GetAllUsers();
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
        Task<UserModel> GetUserEmployeeRoleListById(int userid);
    

        Task<User> GetUserEmployeeByEmpId(UserViewModel model);

        Task<(int statuscode, string message)> UpdateUserAccount(UserViewModel model);
        //Task<(int statuscode, string message, string email)> CreateUserAccount(UserViewModel model);
        Task<UserViewModel> CreateUserAccount(UserViewModel model);
        Task<User> GetUserByEmployeeId(int empId);

        Task<UserManager> GetUserManagerByEmail(string email);
       
        
        // Task<User> GetUserByEmployeeNo(string empNo);
    }
}
