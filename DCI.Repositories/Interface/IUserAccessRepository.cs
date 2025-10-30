using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
    public interface IUserAccessRepository
    {
        Task SaveUserAccess(UserViewModel model);
        Task<UserAccess> GetUserAccessByUserId(int userId);
        Task UpdateUserAccess(UserAccess usr);
        Task<(int statuscode, string message)> ValidateToken(string token);
        Task<(int statuscode, string message)> ChangePassword(ChangePasswordViewModel pass);
		Task SaveExternalUserAccess(int userId);
        Task UpdateUserEmployeeAccess(UserViewModel usr, string token);      

    }
}
