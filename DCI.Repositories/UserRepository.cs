using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Models.Configuration;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace DCI.Repositories
{
	public class UserRepository : IUserRepository, IDisposable
	{
		private readonly DCIdbContext _dbContext;
		private readonly SMTPModel _smtpSettings;
		IUserAccessRepository _useraccessrepository;
		public UserRepository(DCIdbContext context, IUserAccessRepository useraccessrepository)
		{
			this._dbContext = context;
			_useraccessrepository = useraccessrepository;
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<User> GetUserById(int userid)
		{
			return await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == userid);
		}

		public async Task<UserModel> GetUserRoleListById(int userid)
		{
			try
			{	
				UserModel usermodel = new UserModel();

				var context = _dbContext.User.AsQueryable();			

				var query = from usr in context
							where usr.UserId == userid
							select new UserModel
							{
								UserId = usr.UserId,						
								Lastname = usr.Lastname,
								Firstname = usr.Firstname,
								Email = usr.Email,
								ContactNo = usr.ContactNo,
								RoleId = usr.RoleId,
								RoleList = null

							};

				var result = query.FirstOrDefault();

				var rolexList = _dbContext.Role.AsQueryable().ToList();

				if(result == null)
				{
					result = new UserModel(); 
				}
				result.RoleList = rolexList.Count() > 0 ? rolexList : null;
				result.EmployeeList = context.ToList(); //_dbContext.User.AsQueryable().ToList();
				return result;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return null;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<IList<User>> GetAllUsers()
		{
			return await _dbContext.User.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
		}

		public async Task<(int statuscode, string message)> Registration(RegistrationViewModel model)
		{
			try
			{
				if (model.UserId == 0)
				{
					User user = new User();
					user.Email = model.Email;
					user.Firstname = model.Firstname;
					user.Lastname = model.Lastname;
					user.ContactNo = model.ContactNo;		
					user.RoleId = model.RoleId;
					user.DateCreated = DateTime.Now;
					user.CreatedBy = "Admin";
					user.DateModified = DateTime.Now;
					user.ModifiedBy = "Admin";
					user.IsActive = true;
					await _dbContext.User.AddAsync(user);
					await _dbContext.SaveChangesAsync();

					model.UserId = user.UserId;
					await _useraccessrepository.SaveUserAccess(model);
					return (StatusCodes.Status200OK, "Registration successful");
				}
				else
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid User Id");
				}
				//else
				//{
				//	var entities = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == model.UserId);

				//	entities.Username = model.Username;
				//	entities.Firstname = model.Firstname;
				//	entities.Lastname = model.Lastname;
				//	entities.ContactNo = model.ContactNo;
				//	entities.Email = model.Email;
				//	entities.RoleId = model.RoleId;
				//	entities.DateCreated = entities.DateCreated;
				//	entities.CreatedBy = entities.CreatedBy;
				//	entities.DateModified = DateTime.Now;
				//	entities.ModifiedBy = "Admin";
				//	entities.IsActive = true;

				//	_dbContext.User.Entry(entities).State = EntityState.Modified;
				//	await _dbContext.SaveChangesAsync();
				//	return (StatusCodes.Status200OK, "Registration updated successfully");
				//}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status406NotAcceptable, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
		public async Task<(int statuscode, string message)> UpdateUser(UpdateUserViewModel model)
		{
			try
			{
				var entities = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == model.UserId);

		
				entities.Firstname = model.Firstname;
				entities.Lastname = model.Lastname;
				entities.ContactNo = model.ContactNo;
				entities.Email = model.Email;
				entities.RoleId = model.RoleId;
				entities.DateCreated = entities.DateCreated;
				entities.CreatedBy = entities.CreatedBy;
				entities.DateModified = DateTime.Now;
				entities.ModifiedBy = "Admin";
				entities.IsActive = true;

				_dbContext.User.Entry(entities).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return (StatusCodes.Status200OK, "Registration updated successfully");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status406NotAcceptable, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}


		public async Task<bool> IsExistsUsername(string Email)
		{
			return await _dbContext.User.AnyAsync(x => x.Email == Email && x.IsActive == true);
		}
		public async Task<User> GetUserByEmail(string email)
		{
			return await _dbContext.User.FirstOrDefaultAsync(x => x.Email == email);
		}

		public async Task<LoginViewModel> Login(string Email)
		{
			var context = _dbContext.User.AsQueryable();
			var userAccessdbContext = _dbContext.UserAccess.AsQueryable();

			var query = from usr in context
						join usraccess in userAccessdbContext
						on usr.UserId equals usraccess.UserId
						where usr.Email == Email
						select new LoginViewModel
						{
							Email = usr.Email,
							Password = usraccess.Password
						};

			return await query.FirstOrDefaultAsync();
		}

		public async Task<(int statuscode, string message)> Delete(int id)
		{
			try
			{
				var entity = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == id);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid User Id");
				}
				entity.IsActive = false;
				entity.ModifiedBy = "Admin";
				entity.DateModified = DateTime.Now;
				_dbContext.User.Entry(entity).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return (StatusCodes.Status200OK, "Successfully deleted");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status400BadRequest, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<(int statuscode, string message)> SaveExternalUser(ExternalUserModel model)
		{
			try
			{
				var external = await _dbContext.UserExternal.FirstOrDefaultAsync(x => x.Identifier == model.Identifier);
				if (external == null)
				{
					User user = new User();
					user.Email = model.Email;
					user.Firstname = model.Firstname;
					user.Lastname = model.Lastname;
					user.ContactNo = string.Empty;					
					user.RoleId = (int)EnumRole.User;
					user.DateCreated = DateTime.Now;
					user.CreatedBy = "Admin";
					user.DateModified = DateTime.Now;
					user.ModifiedBy = "Admin";
					user.IsActive = true;
					await _dbContext.User.AddAsync(user);
					await _dbContext.SaveChangesAsync();

					await _useraccessrepository.SaveExternalUserAccess(user.UserId);
					return (StatusCodes.Status200OK, "Registration successful");
				}
				return (StatusCodes.Status406NotAcceptable, "Error");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status406NotAcceptable, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
		public async Task<User> GetUserByUsername(string username)
		{
			return _dbContext.User.FirstOrDefault(x => x.Email == username);
		}		
	}
}
