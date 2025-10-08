using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace DCI.WebApp.Services
{
    public class NotificationHub : Hub
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

        public NotificationHub(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)//, /IServiceScopeFactory scopeFactory)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
            // _scopeFactory = scopeFactory;
        }

        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            await base.OnConnectedAsync();
        }

        public async Task GetNotifications()
        {

            List<NotificationViewModel> model = new List<NotificationViewModel>();

            using (var _httpclient = new HttpClient())
            {
                NotificationViewModel _filterRoleModel = new NotificationViewModel();
                var currentUser = _userSessionHelper.GetCurrentUser();
                if (currentUser == null)
                    new List<NotificationViewModel>();

                _filterRoleModel.AssignId = currentUser.UserId;
                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Home/GetAllNotification");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<NotificationViewModel>>(responseBody)!;
                }
            }
            await Clients.Caller.SendAsync("ReceiveNotifications", model.OrderByDescending(x => x.NotificationId).Take(4).ToList());
        }

        public async Task MarkAsRead(int notificationId)
        {
            using (var _httpclient = new HttpClient())
            {
                NotificationViewModel model = new NotificationViewModel();

                model.NotificationId = notificationId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Home/MarkAsRead");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                await Clients.Caller.SendAsync("ReceiveNotifications", "");
            }
        }
    }
}
