using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;
using System.Net.Mail;

namespace DCI.Trigger
{
    public class EmailRepository : IEmailRepository, IDisposable
    {
        private readonly SMTPModel _smtpSettings;
        private DestinationDbContext _dbContext;

        //public EmailRepository(SMTPModel smtpSetting)
        //{
        //    _smtpSettings = smtpSetting;
        //    _apiconfig = apiconfig;
        //}

        public EmailRepository(DestinationDbContext context, IOptions<SMTPModel> smtpSettings)
        {
            _dbContext = context;
            _smtpSettings = smtpSettings.Value;
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

        #region Send Email Biometrics Notification 

        public async Task SendEmailBiometricsNotification(BiometricViewModel model)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = "DCI ESS - Biometric Attendance Logged";
            mail.Body = await SetEmailBiometricsNotification(model);
            mail.IsBodyHtml = true;
            mail.To.Add(model.Email);
            await SendMessage(mail);
        }
        async Task<string> SetEmailBiometricsNotification(BiometricViewModel model)
        {
            try
            {
                string emailbody = $@"
                <html>
                <body>              
                    <p>Hi {model.Fullname},</p>                
                   <p>This is an automated message from ESS System.</p>

                    <p>Your biometric attendance has been recorded as follows:</p>

                     <p>Date: {model.DateTimeInOut.ToLongDateString()}</p>
				     <p>Time: {model.DateTimeInOut.ToString("HH:mm:ss")}</p>
                     <p>Source: BIOMETRICS </p>
                
                    <br/>
                    <p>If you encounter any issues, feel free to contact our support team at info@dci.ph.</p>            
                    <p>Best regards,<br />ESS System Administrator</p>
                </body>
                </html>";

                return emailbody;
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
