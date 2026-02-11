using DCI.Core.Common;
using DCI.Data;
using DCI.PMS.Models.Entities;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Repository
{
    public class HomeRepository : IHomeRepository, IDisposable
    {
        private readonly DCIdbContext _dbContext;
        private readonly PMSdbContext _pmsdbContext;
        public HomeRepository(DCIdbContext context, PMSdbContext pmsContext)
        {
            this._dbContext = context;
            this._pmsdbContext = pmsContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
            _pmsdbContext.Dispose();
        }

        public async Task<DashboardViewModel> GetDashboard()
        {
            DashboardViewModel model = new DashboardViewModel();

            var project = _pmsdbContext.Project.AsNoTracking().Where(x => x.IsActive == true).ToList();

 
            var _projectList =  await (from p in _pmsdbContext.Project.AsNoTracking()
                          join m in _pmsdbContext.Milestone.AsNoTracking() on p.ProjectCreationId equals m.ProjectCreationId                       
                          join c in _pmsdbContext.Client on p.ClientId equals c.ClientId
                          join s in _pmsdbContext.Status on m.Status equals s.StatusId
                          where p.IsActive == true
                          select new MilestoneViewModel
                          {
                              ProjectCreationId = p.ProjectCreationId,                            
                              ProjectName = p.ProjectName,                              
                              MilestoneName = m.MilestoneName, 
                              Percentage = m.Percentage,
                              IsActive = p.IsActive,
                            Status = m.Status,
                              StatusName = s.StatusName,
                              DateModified = m.DateModified
                          }).ToListAsync();

            model.ProjectList = _projectList;
            model.TotalProject = _projectList.Count();
            model.TotalNotStarted = _projectList.Where(x => x.Status == (int)EnumPMSStatus.NotStarted).Count();
            model.TotalInProgress = _projectList.Where(x => x.Status == (int)EnumPMSStatus.InProgress).Count();
            model.TotalCompleted = _projectList.Where(x => x.Status == (int)EnumPMSStatus.Completed).Count();

            return model;
        }
    }
}
