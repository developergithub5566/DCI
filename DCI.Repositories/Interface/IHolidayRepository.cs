using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IHolidayRepository : IDisposable
    {
        Task<HolidayViewModel> GetHolidayById(int holidayId);
        Task<IList<HolidayViewModel>> GetAllHoliday();
        Task<bool> IsExistsHoliday(int holidayId);
        Task<(int statuscode, string message)> Save(HolidayViewModel model);
        Task<(int statuscode, string message)> Delete(HolidayViewModel model);
    }
}
