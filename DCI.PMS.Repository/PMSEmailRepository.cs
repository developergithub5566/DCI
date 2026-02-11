using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Serilog;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace DCI.PMS.Repository
{
    public class PMSEmailRepository : IPMSEmailRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly SMTPModel _smtpSettings;
        //private IUserRepository _userRepository;
        //private IUserAccessRepository _userAccessRepository;
        private readonly IOptions<APIConfigModel> _apiconfig;

        public PMSEmailRepository(DCIdbContext context, IOptions<SMTPModel> smtpSettings, IOptions<APIConfigModel> apiconfig)
        {
            this._dbContext = context;
            _smtpSettings = smtpSettings.Value;
            //_userRepository = userRepository;
            //_userAccessRepository = userAccessRepository;
            this._apiconfig = apiconfig;
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

          //  var userEntity = await _userRepository.GetUserByEmail(email);

           // var userAccessEntity = await _userAccessRepository.GetUserAccessByUserId(userEntity.UserId);


            string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
            string emailBody = $@"
            <html>
            <body>
                <h2>Reset Password Request</h2>
                <p>Hi {"userEntity.Fullname"},</p>
                <p>You requested a password reset. Please click the link below to reset your password:</p>              
                <a href='{link + token}'>Reset Password</a>
                <p>This link will expire on {DateTime.UtcNow.AddDays(1).ToShortDateString()}.</p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Thank you,<br />Your DCI</p>
            </body>
            </html>";

            //userAccessEntity.PasswordResetToken = token;
            //userAccessEntity.PasswordResetTokenExpiry = DateTime.UtcNow.AddDays(1);
      
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
                var token = TokenGeneratorHelper.GetToken();

                string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
                string emailBody = $@"
				<html>
				<body>
					<h2>Welcome to DCI Employee Self-Service</h2>
					<p>Hi {model.Fullname},</p>
					<p>Your account has been successfully created. To activate your account, please click the link below to set your password:</p>              
					<a href='{link + token}'>Set Password</a>
					<p>Please note: This link will expire on {DateTime.UtcNow.AddDays(1).ToShortDateString()} for security purposes.</p>
					<p>If you have any questions or need assistance, feel free to reach out to our support team.</p>
					<p>Thank you and welcome aboard!</p>
					<p>Best regards,<br />ESS System Administrator</p>
				</body>
				</html>";

              //  await _userAccessRepository.UpdateUserEmployeeAccess(model, token);
                return emailBody;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (string.Empty);
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
        #endregion




    }
}
