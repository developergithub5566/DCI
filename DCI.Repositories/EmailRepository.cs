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
using System.Globalization;
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

        public EmailRepository(DCIdbContext context, IOptions<SMTPModel> smtpSettings, IUserRepository userRepository,
            IUserAccessRepository userAccessRepository, IOptions<APIConfigModel> apiconfig)
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

          
            string link = _apiconfig.Value.WebAppConnection + "Account/ValidateToken?token=";
            string emailBody = $@"
            <html>
            <body>
                <h2>Reset Password Request</h2>
                <p>Hi {userEntity.Fullname},</p>
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

                await _userAccessRepository.UpdateUserEmployeeAccess(model, token);
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


        #region Leave
        public async Task SendToApprovalLeave(LeaveViewModel model)
        {
            model = await ApprovalNotificationBodyMessage(model);

            string _leavetype = FormatHelper.GetLeaveTypeName(model.LeaveTypeId);
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            //mail.Subject = System.String.Format("DCI ESS - Action Required: Please check the {0} Request {1}", textInfo.ToTitleCase(_leavetype.ToLower()), model.LeaveRequestHeader.RequestNo);
            mail.Subject = $"DCI ESS - Action Required: Please check the {textInfo.ToTitleCase(_leavetype.ToLower())} Request" + " " + model.LeaveRequestHeader.RequestNo;
            
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.ApproverEmail);
            await SendMessage(mail);
        }

        async Task<LeaveViewModel> ApprovalNotificationBodyMessage(LeaveViewModel model)
        {
            var userEntity = new User();       

            if (model.LeaveRequestHeader.Status == (int)EnumStatus.ForApproval)
            {
                userEntity = await _userRepository.GetUserById(model.ApproverId ?? default(int));                
            }
            model.ApproverEmail = userEntity.Email;
            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
               <p>This is an automated message from the <strong>ESS System</strong>.</p>
                 
				<p>You have been assigned { FormatHelper.GetLeaveTypeName(model.LeaveTypeId).ToLower() } request {model.LeaveRequestHeader.RequestNo} for approval. Kindly review and proceed accordingly.</p>             

                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' { _apiconfig.Value.WebAppConnection }' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }

        public async Task SendToRequestorLeave(LeaveViewModel model)
        {
            model.StatusName = model.LeaveRequestHeader.Status == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;

            model = await RequestorNotificationBodyMessage(model);
            string _leavetype = FormatHelper.GetLeaveTypeName(model.LeaveTypeId);   

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Your {_leavetype.ToLower()} request {model.LeaveRequestHeader.RequestNo} has been {model.StatusName.ToLower()}.";       
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<LeaveViewModel> RequestorNotificationBodyMessage(LeaveViewModel model)
        {
            //var userEntity = await _userRepository.GetUserById(model.LeaveRequestHeader.EmployeeId); 
            var userEntity = await _userRepository.GetUserByEmployeeId(model.LeaveRequestHeader.EmployeeId);
            model.RequestorEmail = userEntity?.Email ?? string.Empty;

            string _leavetype = FormatHelper.GetLeaveTypeName(model.LeaveTypeId);


            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
              <p>This is an automated message from ESS System.</p>
                 <p>Your {_leavetype.ToLower()} request {model.LeaveRequestHeader.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' { _apiconfig.Value.WebAppConnection }' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }

        public async Task SendToLeaveManagement(LeaveViewModel model)
        {
            model = await LeaveManagementNotificationBodyMessage(model);

            string _leavetype = FormatHelper.GetLeaveTypeName(model.LeaveTypeId);
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);   
            mail.Subject = $"DCI ESS - Your {_leavetype.ToLower()} request {model.LeaveRequestHeader?.RequestNo} has been processed.";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<LeaveViewModel> LeaveManagementNotificationBodyMessage(LeaveViewModel model)
        {
            var userEntity = await _userRepository.GetUserByEmployeeId(model.LeaveRequestHeader.EmployeeId);
            model.RequestorEmail = userEntity?.Email ?? string.Empty;

            string _leavetype = FormatHelper.GetLeaveTypeName(model.LeaveTypeId);
            var preparedByEntity = await _userRepository.GetUserById(model.ApproverId ?? 0);
            model.PreparedBy = preparedByEntity?.Fullname ?? string.Empty;


            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
              <p>This is an automated message from ESS System.</p>
                 <p>Your {_leavetype.ToLower()} request {model.LeaveRequestHeader.RequestNo} has been approved and processed by { model.PreparedBy }.</p>   
              
                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }
        #endregion

        #region Overtime

        public async Task SentToApprovalOvertime(OvertimeViewModel model)
        {
            model = await OvertimeNotificationBodyMessage(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Action Required: Please check the Overtime Request {model.RequestNo}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.ApproverEmail);
            await SendMessage(mail);
        }

        async Task<OvertimeViewModel> OvertimeNotificationBodyMessage(OvertimeViewModel model)
        {
            var userEntity = new User();         
          
            if (model.StatusId == (int)EnumStatus.ForApproval)
            {
                userEntity = await _userRepository.GetUserById(model.ApproverId);              
            }

            model.ApproverEmail = userEntity.Email;
            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                      <p>This is an automated message from ESS System.</p>
                 
				<p>You have been assigned overtime request {model.RequestNo} for approval. Kindly review and proceed accordingly. </p> 
            
                 <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }

        public async Task SendToRequestorOT(OvertimeViewModel model)
        {
            model.StatusName = model.StatusId == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;

            model = await RequestorNotificationBodyMessageOT(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Your Overtime request {model.RequestNo} has been {model.StatusName.ToLower()}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<OvertimeViewModel> RequestorNotificationBodyMessageOT(OvertimeViewModel model)
        {
            var userEntity = await _userRepository.GetUserById(model.CreatedBy);
            model.RequestorEmail = userEntity.Email;

            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                <p>This is an automated message from ESS System.</p>
                <p>Your overtime request {model.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }


        #endregion


        #region DTRCorrection

        public async Task SentToApprovalDTRAdjustment(DTRCorrectionViewModel model)
        {
            model = await DTRAdjustmentNotificationBodyMessage(model);


            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Action Required: Please check the DTR Adjustment Request {model.RequestNo}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.ApproverEmail);
            await SendMessage(mail);
        }

        async Task<DTRCorrectionViewModel> DTRAdjustmentNotificationBodyMessage(DTRCorrectionViewModel model)
        {
            var userEntity = new User();        

            if (model.Status == (int)EnumStatus.ForApproval)
            {
                userEntity = await _userRepository.GetUserById(model.ApproverId);              
            }

            model.ApproverEmail = userEntity.Email;
            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                <p>This is an automated message from ESS System.</p>
                 
				<p>You have been assigned DTR Adjustment request {model.RequestNo} for approval. Kindly review and proceed accordingly. </p> 

                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }


        public async Task SendToRequestorDTRAdjustment(DTRCorrectionViewModel model)
        {
            model.StatusName = model.Status == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;

            model = await RequestorNotificationBodyMessageDTRAdjustment(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Your DTR Adjustment {model.RequestNo} has been {model.StatusName.ToLower()}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<DTRCorrectionViewModel> RequestorNotificationBodyMessageDTRAdjustment(DTRCorrectionViewModel model)
        {
            var userEntity = await _userRepository.GetUserById(model.CreatedBy);
            model.RequestorEmail = userEntity.Email;

            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                <p>This is an automated message from ESS System.</p>
                 <p>Your DTR Adjustment request {model.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                 <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }

        #endregion

        #region WFH

        public async Task SentToApprovalWFH(WFHHeaderViewModel model)
        {
            model = await WFHNotificationBodyMessage(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Action Required: Please check the WFH Request {model.RequestNo}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.ApproverEmail);
            await SendMessage(mail);
        }

        async Task<WFHHeaderViewModel> WFHNotificationBodyMessage(WFHHeaderViewModel model)
        {
            var userEntity = new User();         
            
            if (model.StatusId == (int)EnumStatus.ForApproval)
            {
                userEntity = await _userRepository.GetUserById(model.ApproverId);               
            }

            model.ApproverEmail = userEntity.Email;
            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                      <p>This is an automated message from ESS System.</p>
                 
				<p>You have been assigned wfh request {model.RequestNo} for approval. Kindly review and proceed accordingly.</p> 
            
                  <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }



        public async Task SendToRequestorWFH(WFHHeaderViewModel model)
        {
            model.StatusName = model.StatusId == (int)EnumStatus.Approved ? Constants.Approval_Approved : Constants.Approval_Disapproved;

            model = await RequestorNotificationBodyMessageWFH(model);

            MailMessage mail = new MailMessage();
            mail.From = new System.Net.Mail.MailAddress(_smtpSettings.FromEmail);
            mail.Subject = $"DCI ESS - Your Work from home {model.RequestNo} has been {model.StatusName.ToLower()}";
            mail.Body = model.EmailBody;
            mail.IsBodyHtml = true;
            mail.To.Add(model.RequestorEmail);
            await SendMessage(mail);
        }

        async Task<WFHHeaderViewModel> RequestorNotificationBodyMessageWFH(WFHHeaderViewModel model)
        {
            var userEntity = await _userRepository.GetUserById(model.CreatedBy);
            model.RequestorEmail = userEntity.Email;

            model.EmailBody = $@"
            <html>
            <body>              
                <p>Hi {userEntity.Fullname},</p>
                
                <p>This is an automated message from ESS System.</p>
                <p>Your Work from home request {model.RequestNo} has been {model.StatusName.ToLower()}.</p>   
              
                <p>
                    You may log in to your account using the link below:<br />
                    <a href=' {_apiconfig.Value.WebAppConnection}' target='_blank' >
                        Click here to log in to the DCI ESS System
                    </a>
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

            return model;
        }

        #endregion


    }
}
