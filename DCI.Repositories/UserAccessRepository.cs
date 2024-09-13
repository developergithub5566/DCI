using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class UserAccessRepository : IUserAccessRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public UserAccessRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<UserAccess> GetUserAccessByUserId(int userId)
		{
			return await _dbContext.UserAccess.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);
		}
		public async Task SaveUserAccess(UserViewModel model)
		{
			try
			{
				UserAccess entities = new UserAccess();
				entities.UserId = model.UserId;
				entities.Password = PasswordHashingHelper.HashPassword(model.Password);
				entities.DateCreated = DateTime.Now;
				entities.ModifiedDate = DateTime.Now;
				entities.IsActive = true;
				await _dbContext.UserAccess.AddAsync(entities);
				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task SaveExternalUserAccess(int userId)
		{
			try
			{
				UserAccess entities = new UserAccess();
				entities.UserId = userId;
				entities.Password = null;
				entities.DateCreated = DateTime.Now;
				entities.ModifiedDate = DateTime.Now;
				entities.IsActive = true;
				await _dbContext.UserAccess.AddAsync(entities);
				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task UpdateUserAccess(UserAccess usr)
		{
			try
			{
				usr.ModifiedDate = DateTime.Now;
				_dbContext.UserAccess.Entry(usr).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<(int statuscode, string message)> ValidateToken(string token)
		{
			try
			{
				var tokenExist = await _dbContext.UserAccess.FirstOrDefaultAsync(x => x.PasswordResetToken == token);
				var email =  _dbContext.User.FirstOrDefault(x => x.UserId == tokenExist.UserId).Email;

				if (tokenExist?.UserId > 0)
				{
					if (tokenExist.PasswordResetTokenExpiry > DateTime.UtcNow && tokenExist.IsActive == true)
					{
						return (StatusCodes.Status200OK, email);
					}
					else if (tokenExist.PasswordResetTokenExpiry > DateTime.UtcNow && tokenExist.IsActive == false)
					{
                        return (StatusCodes.Status205ResetContent, "The token is already used.");
                    }
                    else
					{
						return (StatusCodes.Status401Unauthorized, "Your password has expired. Please reset your password.");
					}
				}
				else
				{
					return (StatusCodes.Status400BadRequest, "The token is invalid.");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return (StatusCodes.Status401Unauthorized, "Your token does not exist");
		}

		public async Task<(int statuscode, string message)> ChangePassword(ChangePasswordViewModel pass)
		{
			try
			{
				var context = _dbContext.User.AsQueryable();
				var userAccessdbContext = _dbContext.UserAccess.AsQueryable();

				var query = from usr in context
							join usraccess in userAccessdbContext
							on usr.UserId equals usraccess.UserId
							where usr.Email == pass.Email
							select new LoginViewModel
							{
								UserId = usr.UserId,
								UserAccessId = usraccess.UserAccessId,
								Email = usr.Email,
								Password = usraccess.Password
							};

				var useraccessEntity = query.Count() > 0 ? query.FirstOrDefault() : null;

				//if (!PasswordHashingHelper.VerifyPassword(pass.CurrentPassword, useraccessEntity.Password))
				//{
				//	return ((int)StatusCodes.Status401Unauthorized, "Invalid Current Password");
				//}
				if (pass.NewPassword != pass.ConfirmPassword)
				{
					return (StatusCodes.Status401Unauthorized, "Passwords do not match.");
				}

				var useraccess = _dbContext.UserAccess.FirstOrDefault(x => x.UserAccessId == useraccessEntity.UserAccessId);
				useraccess.Password = PasswordHashingHelper.HashPassword(pass.NewPassword);
				useraccess.DateCreated = useraccess.DateCreated;
				useraccess.ModifiedDate = DateTime.Now;
				useraccess.PasswordResetToken = null;
				useraccess.PasswordResetTokenExpiry = null;
				useraccess.IsActive = true;

				_dbContext.UserAccess.Entry(useraccess).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();

				return (StatusCodes.Status200OK, "Success");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return (StatusCodes.Status401Unauthorized, "Invalid Current Password");
		}
	}
}
