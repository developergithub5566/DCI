using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace DCI.PMS.WebApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

        public ProjectController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
        }

        //public IActionResult CreateEdit()
        //{
        //    return View();
        //}


        public async Task<IActionResult> CreateEdit(int id)
        {
            try
            {
                ProjectViewModel model = new ProjectViewModel();
               
                    using (var _httpclient = new HttpClient())
                    {

                        model.ProjectCreationId = id;
                        var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetProjectById");
                        request.Content = stringContent;
                        var response = await _httpclient.SendAsync(request);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        ProjectViewModel vm = JsonConvert.DeserializeObject<ProjectViewModel>(responseBody)!;

                        if (response.IsSuccessStatusCode)
                        {                          
                            return View(vm);
                        }
                    return View(vm);
                }
              
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        //public async Task<IActionResult> Milestone(ProjectViewModel model)
        //{
        //    try
        //    {       
        //        using (var _httpclient = new HttpClient())
        //        {
        //            var currentUser = _userSessionHelper.GetCurrentUser();
        //            if (currentUser == null)
        //                return RedirectToAction("Logout", "Account");
        //            // model.CurrentUserId = currentUser.UserId;
        //            //  model.ProjectCreationId = ProjectCreationId;

        //            var data = new MultipartFormDataContent();
        //            data.Add(new StringContent(model.ProjectCreationId.ToString() ?? ""), "ProjectCreationId");
        //            data.Add(new StringContent(model.ClientId.ToString() ?? ""), "ClientId");

        //            data.Add(new StringContent(model.ProjectNo.ToString() ?? ""), "ProjectNo");
        //            data.Add(new StringContent(model.ProjectName.ToString() ?? ""), "ProjectName");
        //            data.Add(new StringContent(model.NOADate.ToString() ?? ""), "NOADate");
        //            data.Add(new StringContent(model.NTPDate.ToString() ?? ""), "NTPDate");        
        //            data.Add(new StringContent(model.MOADate.ToString() ?? ""), "MOADate");
        //            data.Add(new StringContent(model.ProjectDuration.ToString() ?? ""), "ProjectDuration");
        //            data.Add(new StringContent(model.ProjectCost.ToString() ?? ""), "ProjectCost");
        //            data.Add(new StringContent(model.ModeOfPayment.ToString() ?? ""), "ModeOfPayment");
        //            data.Add(new StringContent(model.Status.ToString() ?? ""), "Status");
        //            data.Add(new StringContent(model.CreatedName.ToString() ?? ""), "CreatedName");
        //            //data.Add(new StringContent(model.DateCreated.ToString() ?? ""), "DateCreated");
        //            //data.Add(new StringContent(model.CreatedBy.ToString() ?? ""), "CreatedBy");
        //            //data.Add(new StringContent(model.ModifiedBy.ToString() ?? ""), "ModifiedBy");
        //            //data.Add(new StringContent(model.DateModified.ToString() ?? ""), "DateModified");
        //            data.Add(new StringContent(model.IsActive.ToString() ?? ""), "IsActive");

        //            if (model.NOAFile != null)
        //            {
        //                var fileContent = new StreamContent(model.NOAFile!.OpenReadStream());
        //                data.Add(fileContent, "NOAFile", model.NOAFile.FileName);
        //            }
        //            if (model.NTPFile != null)
        //            {
        //                var fileContent = new StreamContent(model.NTPFile!.OpenReadStream());
        //                data.Add(fileContent, "NTPFile", model.NTPFile.FileName);
        //            }

        //            if (model.MOAFile is not null)
        //            {
        //                var fileContent = new StreamContent(model.MOAFile!.OpenReadStream());
        //                data.Add(fileContent, "MOAFile", model.MOAFile.FileName);
        //            }           


        //            var response = await _httpclient.PostAsync(_apiconfig.Value.apiPMS + "api/Project/SaveProject", data);
        //            var responseBody = await response.Content.ReadAsStringAsync();


        //        }



        //        using (var _httpclient = new HttpClient())
        //        {
        //            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        //            var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetMilestoneByProjectId");
        //            request.Content = stringContent;
        //            var response = await _httpclient.SendAsync(request);
        //            var responseBody = await response.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<ProjectViewModel>(responseBody)!;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                return View(model);
        //            }
        //            return View(model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //    return Json(new { success = false, message = "An error occurred. Please try again." });
        //}


        public async Task<IActionResult> SaveProject(ProjectViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    // model.CurrentUserId = currentUser.UserId;
                    //  model.ProjectCreationId = ProjectCreationId;

                    var data = new MultipartFormDataContent();
                    data.Add(new StringContent(model.ProjectCreationId.ToString() ?? ""), "ProjectCreationId");
                    data.Add(new StringContent(model.ClientId.ToString() ?? ""), "ClientId");

                    data.Add(new StringContent(model.ProjectNo.ToString() ?? ""), "ProjectNo");
                    data.Add(new StringContent(model.ProjectName.ToString() ?? ""), "ProjectName");
                    data.Add(new StringContent(model.NOADate.ToString() ?? ""), "NOADate");
                    data.Add(new StringContent(model.NTPDate.ToString() ?? ""), "NTPDate");
                    data.Add(new StringContent(model.MOADate.ToString() ?? ""), "MOADate");
                    data.Add(new StringContent(model.ProjectDuration.ToString() ?? ""), "ProjectDuration");
                    data.Add(new StringContent(model.ProjectCost.ToString() ?? ""), "ProjectCost");
                    data.Add(new StringContent(model.ModeOfPayment.ToString() ?? ""), "ModeOfPayment");
                    data.Add(new StringContent(model.Status.ToString() ?? ""), "Status");
                    data.Add(new StringContent(model.CreatedName.ToString() ?? ""), "CreatedName");
                    //data.Add(new StringContent(model.DateCreated.ToString() ?? ""), "DateCreated");
                    //data.Add(new StringContent(model.CreatedBy.ToString() ?? ""), "CreatedBy");
                    //data.Add(new StringContent(model.ModifiedBy.ToString() ?? ""), "ModifiedBy");
                    //data.Add(new StringContent(model.DateModified.ToString() ?? ""), "DateModified");
                    data.Add(new StringContent(model.IsActive.ToString() ?? ""), "IsActive");

                    if (model.NOAFile != null)
                    {
                        var fileContent = new StreamContent(model.NOAFile!.OpenReadStream());
                        data.Add(fileContent, "NOAFile", model.NOAFile.FileName);
                    }
                    if (model.NTPFile != null)
                    {
                        var fileContent = new StreamContent(model.NTPFile!.OpenReadStream());
                        data.Add(fileContent, "NTPFile", model.NTPFile.FileName);
                    }

                    if (model.MOAFile is not null)
                    {
                        var fileContent = new StreamContent(model.MOAFile!.OpenReadStream());
                        data.Add(fileContent, "MOAFile", model.MOAFile.FileName);
                    }


                    var response = await _httpclient.PostAsync(_apiconfig.Value.apiPMS + "api/Project/SaveProject", data);
                    var responseBody = await response.Content.ReadAsStringAsync();


                }

                return RedirectToAction("Milestone", new { projectCreationId = model.ProjectCreationId });
         

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Milestone(int projectCreationId)
        {
            try
            {

                ProjectViewModel model = new ProjectViewModel();
                model.ProjectCreationId = projectCreationId;

                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetMilestoneByProjectId");
                    request.Content = stringContent;
                    var response =  await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProjectViewModel>(responseBody)!;

                    if (response.IsSuccessStatusCode)
                    {
                        return View(model);
                    }
                    //return View(model);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
        public async Task<IActionResult> AddMilestone(MilestoneViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                  

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/SaveMilestone");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Deliverables(MilestoneViewModel model)
        {
         
            using (var _httpclient = new HttpClient())
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetDeliverablesByMilestoneId");
                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<MilestoneViewModel>(responseBody)!;

                if (response.IsSuccessStatusCode)
                {
                    return View(model);
                }
                return View(model);
            }
        }

        public async Task<IActionResult> AddDeliverable(DeliverableViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/SaveDeliverable");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Index(ProjectViewModel model)
        {
            try
            {
                List<ProjectViewModel> list = new List<ProjectViewModel>();
                using (var _httpclient = new HttpClient())
                {

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetAllProject");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        list = JsonConvert.DeserializeObject<List<ProjectViewModel>>(responseBody)!;
                    }
                    return View(list);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                //Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }
}
