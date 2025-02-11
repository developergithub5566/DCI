using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Core.Common;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Serilog;
using static System.Net.WebRequestMethods;
using DCI.Models.ViewModel;
using System.Reflection.Metadata;
using Microsoft.Identity.Client;
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


		#region Upload File
		public async Task SendUploadFile(DocumentViewModel model)
		{
			model = await UploadFileBodyMessage(model);

			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "Action Required: Please Upload Your Document No. " + model.DocNo;
			mail.Body = model.EmailBody;
			mail.IsBodyHtml = true;
			mail.To.Add(model.RequestByEmail);
			await SendMessage(mail);
		}

		async Task<DocumentViewModel> UploadFileBodyMessage(DocumentViewModel model)
		{
			var userEntity = await _userRepository.GetUserById(model.RequestById ?? default(int));

			//string link = "http://192.168.1.78:83/Document/Upload?token=";
			string link = _apiconfig.Value.WebAppConnection + "Document/Upload?token=";
			model.RequestByEmail = userEntity.Email;
			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p> Please upload the required document (Document No: {model.DocNo}) by following the link below:  </p>   
                 <a href='{link + model.UploadLink}'> Upload file</a>
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";


			return model;
		}
		#endregion


		#region Set Password
		public async Task SendSetPassword(string email)
		{
			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = Constants.Email_Subject_SetPassword;
			mail.Body = await SetPasswordBodyMessage(email);
			mail.IsBodyHtml = true;
			mail.To.Add(email);
			await SendMessage(mail);
		}
		async Task<string> SetPasswordBodyMessage(string email)
		{
			var token = TokenGeneratorHelper.GetToken();

			var userEntity = await _userRepository.GetUserByEmail(email);

			var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(userEntity.UserId);

			//string link = "http://192.168.1.78:83/Account/ValidateToken?token=";
			string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
			string emailBody = $@"
            <html>
            <body>
                <h2>Set Password Request</h2>
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                <p>Your account has been successfully created! Please click the link below to set your password:</p>              
                <a href='{link + token}'>Set Password</a>
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

		#region Approval
		public async Task SendApproval(DocumentViewModel model)
		{
			model = await ApprovalNotificationBodyMessage(model);


			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "Action Required: Please check the document no. " + model.DocNo;
			mail.Body = model.EmailBody;
			mail.IsBodyHtml = true;
			mail.To.Add(model.RequestByEmail);
			await SendMessage(mail);
		}

		async Task<DocumentViewModel> ApprovalNotificationBodyMessage(DocumentViewModel model)
		{
			var userEntity = new User();
			string statusName = string.Empty;

			if (model.StatusId == (int)EnumDocumentStatus.ForReview)
			{
				userEntity = await _userRepository.GetUserById(model.Reviewer ?? default(int));
				statusName = "for review";
			}

			if (model.StatusId == (int)EnumDocumentStatus.ForApproval)
			{
				userEntity = await _userRepository.GetUserById(model.Approver ?? default(int));
				statusName = "for approval";
			}

			string link = _apiconfig.Value.WebAppConnection + "Document/Details?DocId=" + model.DocId;
			model.RequestByEmail = userEntity.Email;
			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p>Please check the document (Document No:<a href='{link}'> {model.DocNo}</a>) {statusName}. </p>   
            
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			return model;
		}

		public async Task SendRequestor(DocumentViewModel model, ApprovalViewModel apprvm)
		{
			if(!apprvm.Action)
			{
				model = await DisapprovedReuploadBodyMessage(model, apprvm);
			}
			else
			{
				model = await RequestorNotificationBodyMessage(model, apprvm);
			}


            if (model.StatusId == (int)EnumDocumentStatus.Approved)
            {
                apprvm.ApprovalStatus = apprvm.Action == true ? Constants.Approval_Approved : Constants.Approval_Disapproved;
            }
            else if (model.StatusId == (int)EnumDocumentStatus.ForApproval)
            {
                apprvm.ApprovalStatus = apprvm.Action == true ? Constants.Approval_Reviewed : Constants.Approval_Disapproved;
            }


            MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "DCI App - Your document No. " + model.DocNo + " has been " + apprvm.ApprovalStatus.ToLower();
			mail.Body = model.EmailBody;
			mail.IsBodyHtml = true;
			mail.To.Add(model.RequestByEmail);
			await SendMessage(mail);
		}

		async Task<DocumentViewModel> RequestorNotificationBodyMessage(DocumentViewModel model, ApprovalViewModel apprvm)
		{
			var userEntity = await _userRepository.GetUserById(model.RequestById ?? default(int));


			if (model.StatusId == (int)EnumDocumentStatus.Approved)
			{
				apprvm.ApprovalStatus = apprvm.Action == true ? Constants.Approval_Approved : Constants.Approval_Disapproved;
			}
			else if (model.StatusId == (int)EnumDocumentStatus.ForApproval)
			{
				apprvm.ApprovalStatus = apprvm.Action == true ? Constants.Approval_Reviewed : Constants.Approval_Disapproved;
			}

			if (model.StatusId == (int)EnumDocumentStatus.InProgress && !apprvm.Action)
			{
				apprvm.ApprovalStatus = Constants.Approval_Disapproved;
			}

			string link = _apiconfig.Value.WebAppConnection + "Document/Details?DocId=" + model.DocId;
            model.RequestByEmail = userEntity.Email;
			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p>The document (Document No: <a href='{link}'> {model.DocNo}</a>)  has been {apprvm.ApprovalStatus.ToLower()}.</p>   
              
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			return model;
		}

		async Task<DocumentViewModel> DisapprovedReuploadBodyMessage(DocumentViewModel model, ApprovalViewModel apprvm)
		{
			var userEntity = await _userRepository.GetUserById(model.RequestById ?? default(int));

			apprvm.ApprovalStatus = Constants.Approval_Disapproved;

			string link = _apiconfig.Value.WebAppConnection + "Document/Upload?token=";
			model.RequestByEmail = userEntity.Email;
			model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p>The document (Document No: <a href='{link}'> {model.DocNo}</a>)  has been {apprvm.ApprovalStatus.ToLower()}.</p>   
				 <p>Please upload the updated document. </p> <a href='{link + model.UploadLink}'>Upload File</a>

                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			return model;
		}



		#endregion
	}
}
