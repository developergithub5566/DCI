using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Serilog;
using System.Net;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.Trigger
{
    public class EmailRepository : IEmailRepository, IDisposable
    {
        private readonly SMTPModel _smtpSettings;
        private DestinationDbContext _dbContext;
        private readonly IOptions<APIConfigModel> _apiconfig;

        //public EmailRepository(SMTPModel smtpSetting)
        //{
        //    _smtpSettings = smtpSetting;
        //    _apiconfig = apiconfig;
        //}

        public EmailRepository(DestinationDbContext context, IOptions<SMTPModel> smtpSettings, IOptions<APIConfigModel> apiconfig)
        {
            _dbContext = context;
            _smtpSettings = smtpSettings.Value;
            _apiconfig = apiconfig;
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
                //       string emailbody = $@"
                //       <html>
                //       <body>              
                //           <p>Hi {model.Fullname},</p>                
                //          <p>This is an automated message from ESS System.</p>

                //           <p>Your biometric attendance has been recorded as follows:</p>

                //            <p>Date: {model.DateTimeInOut.ToLongDateString()}</p>
                //<p>Time: {model.DateTimeInOut.ToString("HH:mm:ss")}</p>
                //            <p>Source: BIOMETRICS </p>

                //           <br/>
                //           <p>If you encounter any issues, feel free to contact our support team at info@dci.ph.</p>            
                //           <p>Best regards,<br />ESS System Administrator</p>
                //       </body>
                //       </html>";

                string emailBody = $@"
                                <html>
                                  <body style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                                    <p>Hi {model.Fullname},</p>
    
                                    <p>This is an automated message from the <strong>ESS System</strong>.</p>

                                    <p>Your biometric attendance has been recorded as follows:</p>
                                    <table style='border-collapse: collapse;'>
                                      <tr><td style='padding: 2px 8px;'>Date:</td><td>{model.DateTimeInOut.ToLongDateString()}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>Time:</td><td>{model.DateTimeInOut.ToString("HH:mm:ss")}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>Source:</td><td> BIOMETRICS </td></tr>                                     
                                    </table>

                                    <br>

                                    <p style='color:#555;'>                                     
                                        To stop receiving email notifications, turn off the email notification setting in your ESS profile.
                                    </p>

                                    <hr style='border:none; border-top:1px solid #ddd; margin:20px 0;' />

                                    <p style='font-size:13px; color:#777;'>
                                      If you encounter any issues, contact our support team at 
                                      <a href='mailto:info@dci.ph' style='color:#0066cc;'>info@dci.ph</a>.
                                    </p>

                                    <p style='font-size:13px; color:#777;'>
                                      Best regards,<br>
                                      <strong>ESS System Administrator</strong><br>
                                      <span style='color:#999;'>DCI Employee Self-Service Portal</span>
                                    </p>

                                    <p style='font-size:11px; color:#aaa; margin-top:20px;'>
                                      This email was automatically generated. Please do not reply directly to this message.
                                    </p>
                                  </body>
                                </html>";

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

        public async Task SendEmailAttendanceConfirmationNotification(BiometricViewModel model)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Action Required: Confirmation on Attendance | {model.DATE}" ;
            mail.Body = await SetEmailAttendanceConfirmationNotification(model);
            mail.IsBodyHtml = true;
            mail.To.Add(model.Email);
            await SendMessage(mail);
        }

        async Task<string> SetEmailAttendanceConfirmationNotification(BiometricViewModel model)
        {
            try
            {     
                string emailBody = $@"
                                <html>
                                  <body style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                                    <p>Hi {model.Fullname},</p>
    
                                    <p>This is an automated message from the <strong>ESS System</strong>.</p>

                                    <p>Kindly check the details below:</p>
                                    <table style='border-collapse: collapse;'>
                                      <tr><td style='padding: 2px 8px;'>Date:</td><td>{model.DATE}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>First In:</td><td>{model.FIRST_IN}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>Last Out:</td><td>{model.LAST_OUT}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>Status:</td><td>{model.STATUS_STRING}</td></tr>
                                      <tr><td style='padding: 2px 8px;'>Remarks:</td><td><strong>{model.REMARKS}</strong></td></tr>
                                    </table>

                                      <p>
                                        You may log in to your account using the link below:<br />
                                        <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                                            Click here to log in to the DCI ESS System
                                        </a>
                                    </p>

                                    <br>

                                    <p style='color:#555;'>                                     
                                        To stop receiving email notifications, turn off the email notification setting in your ESS profile.
                                    </p>

                                    <hr style='border:none; border-top:1px solid #ddd; margin:20px 0;' />

                                    <p style='font-size:13px; color:#777;'>
                                      If you encounter any issues, contact our support team at 
                                      <a href='mailto:info@dci.ph' style='color:#0066cc;'>info@dci.ph</a>.
                                    </p>

                                    <p style='font-size:13px; color:#777;'>
                                      Best regards,<br>
                                      <strong>ESS System Administrator</strong><br>
                                      <span style='color:#999;'>DCI Employee Self-Service Portal</span>
                                    </p>

                                    <p style='font-size:11px; color:#aaa; margin-top:20px;'>
                                      This email was automatically generated. Please do not reply directly to this message.
                                    </p>
                                  </body>
                                </html>";


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

        public async Task SendEmailAttendanceConfirmationNotificationMonthly(BiometricViewModel model)
        {
            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Action Required: Confirmation on Attendance | {"October 2025"}";
            mail.Body = await SetEmailAttendanceConfirmationNotificationMonthly(model);
            mail.IsBodyHtml = true;
            mail.To.Add(model.Email);
            await SendMessage(mail);
        }

        async Task<string> SetEmailAttendanceConfirmationNotificationMonthly(BiometricViewModel model)
        {
            try
            {
                string emailBody = $@"
                                <html>
                                  <body style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                                    <p>Hi {model.Fullname},</p>
    
                                    <p>This is an automated message from the <strong>ESS System</strong>.</p>

                                    <p>Kindly check the details below:</p>
                                    {model.BODYTABLE}

                                    <p>
                                        You may log in to your account using the link below:<br />
                                        <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                                            Click here to log in to the DCI ESS System
                                        </a>
                                    </p>

                                    <br>                              

                                    <hr style='border:none; border-top:1px solid #ddd; margin:20px 0;' />

                                    <p style='font-size:13px; color:#777;'>
                                      If you encounter any issues, contact our support team at 
                                      <a href='mailto:info@dci.ph' style='color:#0066cc;'>info@dci.ph</a>.
                                    </p>

                                    <p style='font-size:13px; color:#777;'>
                                      Best regards,<br>
                                      <strong>ESS System Administrator</strong><br>
                                      <span style='color:#999;'>DCI Employee Self-Service Portal</span>
                                    </p>

                                    <p style='font-size:11px; color:#aaa; margin-top:20px;'>
                                      This email was automatically generated. Please do not reply directly to this message.
                                    </p>
                                  </body>
                                </html>";


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
