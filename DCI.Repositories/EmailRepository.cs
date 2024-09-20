using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Serilog;
using static System.Net.WebRequestMethods;

namespace DCI.Repositories
{
	public class EmailRepository : IEmailRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private readonly SMTPModel _smtpSettings;
		private IUserRepository _userRepository;
		private IUserAccessRepository _userAccessRepository;
		public EmailRepository(DCIdbContext context, IOptions<SMTPModel> smtpSettings, IUserRepository userRepository, IUserAccessRepository userAccessRepository)
		{
			this._dbContext = context;
			_smtpSettings = smtpSettings.Value;
			_userRepository = userRepository;
			_userAccessRepository = userAccessRepository;
		}
		public async Task<bool> IsExistsEmail(string email)
		{
			return await _dbContext.User.AnyAsync(x => x.Email == email);
		}

		public async Task SendPasswordReset(string email)
		{
			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "Reset Password";
			mail.Body = await SendPasswordResetBodyMessage(email);
			mail.IsBodyHtml = true;
			mail.To.Add(email);
			await SendMessage(mail);
		}

		async Task<string> SendPasswordResetBodyMessage(string email)
		{
			var token = TokenGeneratorHelper.GetToken();

			var userEntity = await _userRepository.GetUserByEmail(email);

			var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(userEntity.UserId);

			string link = "https://localhost:7236/Account/ValidateToken?token=";
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
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task SendUploadFile(string email, string docno)
		{
			MailMessage mail = new MailMessage();
			mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
			mail.Subject = "Action Required: Please Upload Your Document No " + docno;
			mail.Body = await SendUploadFileBodyMessage(email, docno);
			mail.IsBodyHtml = true;
			mail.To.Add(email);
			await SendMessage(mail);
		}

		async Task<string> SendUploadFileBodyMessage(string email, string docno)
		{
			var token = TokenGeneratorHelper.GetToken();

			var userEntity = await _userRepository.GetUserByEmail(email);

			var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(userEntity.UserId);

			string link = "https://localhost:7236/Document/Upload?token=";
			string emailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Firstname + " " + userEntity.Lastname},</p>
                
                 <p>This is an automated message from DCI Application.</p>
                 <p> Please upload the required document (Document No: {docno}) by following the link below:  </p>   
                 <a href='{link + token}'> Upload file</a>
                <p>If you encounter any issues, please contact our support team at [DCI Application Support].</p>            
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

			userAccessEntity.PasswordResetToken = token;
			userAccessEntity.PasswordResetTokenExpiry = DateTime.UtcNow.AddDays(1);
			await _userAccessRepository.UpdateUserAccess(userAccessEntity);
			return emailBody;
		}
	}
}
