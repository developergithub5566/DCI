using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Serilog;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata;
using static System.Net.WebRequestMethods;
//using System.Reflection.Metadata;

namespace DCI.Repositories
{
	public class EmailRepository : IEmailRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private readonly SMTPModel _smtpSettings;
		private IUserRepository _userRepository;
		private IUserAccessRepository _userAccessRepository;
		private readonly IOptions<APIConfigModel> _apiconfig;

		public EmailRepository(DCIdbContext context, IOptions<SMTPModel> smtpSettings, IUserRepository userRepository, IUserAccessRepository userAccessRepository, IOptions<APIConfigModel> apiconfig)
		{
			this._dbContext = context;
			_smtpSettings = smtpSettings.Value;
			_userRepository = userRepository;
			_userAccessRepository = userAccessRepository;
			this._apiconfig = apiconfig;
		}
		public async Task<bool> IsExistsEmail(string email)
		{
			return await _dbContext.User.AnyAsync(x => x.Email == email && x.IsActive == true);
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}
		public async Task SendMessage(MailMessage msg)
		{
			SmtpClient smtp = new SmtpClient();
			try
			{
				smtp.Host = _smtpSettings.Host;
				smtp.Port = _smtpSettings.Port;
				smtp.EnableSsl = _smtpSettings.EnableSsl;
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtp.UseDefaultCredentials = _smtpSettings.UseDefaultCredentials;
				smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
				smtp.Send(msg);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
				smtp.Dispose();
			}
		}


		#region Reset Password
		public async Task SendResetPassword(string email)
		{
			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = Constants.Email_Subject_ResetPassword;
			mail.Body = await ResetPasswordBodyMessage(email);
			mail.IsBodyHtml = true;
			mail.To.Add(email);
			await SendMessage(mail);
		}

		async Task<string> ResetPasswordBodyMessage(string email)
		{
			var token = TokenGeneratorHelper.GetToken();

			var userEntity = await _userRepository.GetUserByEmail(email);

			var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(userEntity.UserId);

			//string link = "http://192.168.1.78:83/Account/ValidateToken?token=";
			string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
			string emailBody = $@"
            <html>
            <body>
                <h2>Reset Password Request</h2>
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                <p>You requested a password reset. Please click the link below to reset your password:</p>              
                <a href='{link + token}'>Reset Password</a>
                <p>This link will expire on {DateTime.UtcNow.AddDays(1).ToShortDateString()}.</p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			userAccessEntity.PasswordResetToken = token;
			userAccessEntity.PasswordResetTokenExpiry = DateTime.UtcNow.AddDays(1);
			await _userAccessRepository.UpdateUserAccess(userAccessEntity);
			return emailBody;
		}

        #endregion





        #region Set Password

        public async Task SendSetPassword(UserViewModel model)     
        {
			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = Constants.Email_Subject_SetPassword;
			mail.Body = await SetPasswordBodyMessage(model);
			mail.IsBodyHtml = true;
			mail.To.Add(model.Email);
			await SendMessage(mail);
		}
		async Task<string> SetPasswordBodyMessage(UserViewModel model)
		{
			try
			{
            

               // var userEntity = await _userRepository.GetUserByEmail(email);

                 var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(model.UserId);

                var token = TokenGeneratorHelper.GetToken();
                //var userAccessEntity = usraccssentity.FirstOrDefault();


                //string link = "http://192.168.1.78:83/Account/ValidateToken?token=";
                string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
                string emailBody = $@"
				<html>
				<body>
					<h2>Welcome to DCI Employee Self-Service</h2>
					<p>Hi {model.Firstname + " " + model.Lastname},</p>
					<p>Your account has been successfully created. To activate your account, please click the link below to set your password::</p>              
					<a href='{link + token}'>Set Password</a>
					<p>Please note: This link will expire on {DateTime.UtcNow.AddDays(1).ToShortDateString()} for security purposes.</p>
					<p>If you have any questions or need assistance, feel free to reach out to our support team.</p>
					<p>Thank you and welcome aboard!</p>
					<p>Best regards,<br />DocTrack System Administrator</p>
				</body>
				</html>";
				//UserAccess userAccessEntity = new UserAccess();
				//userAccessEntity.UserId = model.UserId;
                userAccessEntity.PasswordResetToken = token;
                userAccessEntity.PasswordResetTokenExpiry = DateTime.UtcNow.AddDays(1);
                await _userAccessRepository.UpdateUserEmployeeAccess(userAccessEntity,token);
                return emailBody;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return ( string.Empty);
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
		#endregion

		#region Approval
		public async Task SendToApproval(LeaveViewModel model)
		{
			model = await ApprovalNotificationBodyMessage(model);


			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "Action Required: Please check the Leave Request no. " + model.LeaveRequestHeader.RequestNo;
			mail.Body = model.EmailBody;
			mail.IsBodyHtml = true;
			mail.To.Add(model.ApproverEmail);
			await SendMessage(mail);
		}

		async Task<LeaveViewModel> ApprovalNotificationBodyMessage(LeaveViewModel model)
		{
			var userEntity = new User();
			string statusName = string.Empty;
	

			if (model.LeaveRequestHeader.Status == (int)EnumStatus.ForApproval)
			{
				userEntity = await _userRepository.GetUserById(model.ApproverId ?? default(int));
				statusName = "for approval";
			}

            //string link = _apiconfig.Value.WebAppConnection + "Document/Details?DocId=" + model.DocId;

            //<p>Please check the Leave Request {model.LeaveRequestHeader.RequestNo} {statusName}. </p> 
            model.ApproverEmail = userEntity.Email;
			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 
				<p>You have been assigned leave request {model.LeaveRequestHeader.RequestNo} {statusName}. Kindly review and proceed accordingly. </p> 
            
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			return model;
		}

		public async Task SendToRequestor(LeaveViewModel model)
		{
            model.StatusName = model.LeaveRequestHeader.Status == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;
        
            model = await RequestorNotificationBodyMessage(model);

			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "DCI App - Your leave No. " + model.LeaveRequestHeader.RequestNo + " has been " + model.StatusName.ToLower();
			mail.Body = model.EmailBody;
			mail.IsBodyHtml = true;
			mail.To.Add(model.RequestorEmail);
			await SendMessage(mail);
		}

		async Task<LeaveViewModel> RequestorNotificationBodyMessage(LeaveViewModel model)
		{
			var userEntity = await _userRepository.GetUserById(model.LeaveRequestHeader.EmployeeId); 
			model.RequestorEmail = userEntity.Email;

			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p>Your Leave request no {model.LeaveRequestHeader.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			return model;
		}


        public async Task SendToRequestorDTR(DTRCorrectionViewModel model)
        {
            model.StatusName = model.Status == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;

            model = await RequestorNotificationBodyMessageDTR(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = "DCI App - Your DTR No. " + model.RequestNo + " has been " + model.StatusName.ToLower();
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<DTRCorrectionViewModel> RequestorNotificationBodyMessageDTR(DTRCorrectionViewModel model)
        {
            var userEntity = await _userRepository.GetUserById(model.CreatedBy);
            model.RequestorEmail = userEntity.Email;

            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p>Your DTR correction request no {model.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

            return model;
        }

        #endregion
    }
}
