using DCI.Models.Configuration;
using DCI.PMS.Models.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Data;
using Dapper;


namespace DCI.Trigger
{
    public class ProjectProcessor
    {
        // private readonly PMSDbContext _pmsdbcontext;
        private readonly IDbConnection _essconnection;
        private readonly IDbConnection _pmsconnection;
        IEmailRepository _emailRepository;
        private readonly IOptions<APIConfigModel> _apiconfig;
        public ProjectProcessor(IEmailRepository emailRepository, IOptions<APIConfigModel> apiconfig, IConfiguration pmsconfiguration, IConfiguration essconfiguration)
        {
            // _pmsconnection = pmsconfiguration;
            //  _pmsdbcontext = pmsdbcontext;
            _emailRepository = emailRepository;
            _apiconfig = apiconfig;

            var connStrPMS = pmsconfiguration.GetConnectionString("PMS") ?? throw new InvalidOperationException("Connection string not found.");
            _pmsconnection = new SqlConnection(connStrPMS);

            var connStrESS = essconfiguration.GetConnectionString("SqlServerB") ?? throw new InvalidOperationException("Connection string not found.");
            _essconnection = new SqlConnection(connStrESS);


        }

        public async Task TargetCompletion()
        {
            try
            {
              
                //var sql = @"
                //        SELECT
                //         a.ProjectCreationId,
	               //      a.ProjectName,
	               //      a.ProjectNo,
	               //      b.MilestoneName,
                //         b.MileStoneId,
                //         b.TargetCompletedDate,
                //            EmailDaysBefore =
                //                DATEDIFF(DAY, CAST(GETDATE() AS DATE), CAST(b.TargetCompletedDate AS DATE))
                //        FROM [dbo].[Project] a
                //        LEFT JOIN [dbo].[Milestone] b
                //            ON a.ProjectCreationId = b.ProjectCreationId
                //        WHERE
                //            b.TargetCompletedDate IS NOT NULL
                //            AND DATEDIFF(DAY, CAST(GETDATE() AS DATE), CAST(b.TargetCompletedDate AS DATE))
                //                IN (7,1)
                //    ";

                var sql = @"
                       SELECT
                         a.ProjectCreationId,
	                     a.ProjectName,
	                     a.ProjectNo,
	                     b.MilestoneName,
                         b.MileStoneId,
                         b.TargetCompletedDate,
	                     c.UserId,
                         EmailDaysBefore =
                             DATEDIFF(DAY, CAST(GETDATE() AS DATE), CAST(b.TargetCompletedDate AS DATE))
                     FROM [dbo].[Project] a
                     LEFT JOIN [dbo].[Milestone] b ON a.ProjectCreationId = b.ProjectCreationId
                     LEFT JOIN [dbo].[Coordinator] c on a.ProjectCreationId = c.ProjectCreationId     
                     WHERE
                         b.TargetCompletedDate IS NOT NULL
                         AND DATEDIFF(DAY, CAST(GETDATE() AS DATE), CAST(b.TargetCompletedDate AS DATE))
                             IN (7,1)
                    ";


                var milestones = (await _pmsconnection.QueryAsync<MilestoneViewModel>(sql)).ToList();

                      


                foreach (var item in milestones)
                {
                    var usersql = @"
                        SELECT DISTINCT Email, Fullname
                        FROM [User]
                        WHERE IsActive = 1
                          AND UserId = @UserIds
                    ";

                    item.UserEmailList = (await _essconnection.QueryAsync<UserEmailList>(
                               usersql,
                               new { UserIds = item.UserId }
                           )).ToList();

                    await _emailRepository.SendTargetCompletionDueDate(item);
                    //if (item.EmailDaysBefore == 7)
                    //{
                    //    await _emailRepository.SendTargetCompletionDueDate(item); //ReminderType.SevenDays
                    //}
                    //else if (item.EmailDaysBefore == 1)
                    //{
                    //      await _emailRepository.SendTargetCompletionDueDate(item); // ReminderType.OneDay
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
