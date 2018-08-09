using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using OverrideCustomPage.Helpers;
using OverrideCustomPage.Models;
using Relativity.API;
using Relativity.CustomPages;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OverrideCustomPage.Controllers
{
    public class HomeController : Controller
    {
        public IApiHelper ApiHelper { get; set; }

        public HomeController()
        {
            IAPILog apiLog = ConnectionHelper.Helper().GetLoggerFactory().GetLogger();
            APIOptions rsapiApiOptions = new APIOptions
            {
                WorkspaceID = -1
            };
            IRSAPIClient rsapiClient = ConnectionHelper.Helper().GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
            rsapiClient.APIOptions = rsapiApiOptions;
            IGenericRepository<RDO> rdoRepository = rsapiClient.Repositories.RDO;
            ApiHelper = new RsapiHelper(apiLog, rdoRepository, rsapiApiOptions);
        }

        [HttpGet]
        public async Task<ActionResult> NewRdo(String AppID)
        {
            if (AppID == null)
            {
                throw new ArgumentNullException(nameof(AppID));
            }

            int workspaceArtifactId;
            if (!int.TryParse(AppID, out workspaceArtifactId))
            {
                throw new ArgumentException($"{nameof(AppID)} is not an integer. [Value = {AppID}]");
            }

            NewRdoModel newRdoModel = await Task.Run(() => new NewRdoModel { WorkspaceArtifactId = workspaceArtifactId });
            return View(newRdoModel);
        }

        [HttpPost]
        public async Task<ActionResult> NewRdo(NewRdoModel newRdoModel)
        {
            if (ModelState.IsValid)
            {
                if (newRdoModel == null)
                {
                    throw new ArgumentNullException(nameof(newRdoModel));
                }

                try
                {
                    await ApiHelper.SaveNewValuesAsync(newRdoModel);
                    return View("Confirmation",
                        new ResultModel
                        {
                            Header = Helpers.Constants.ViewHeaderText.NewRdo,
                            Message = $"{Helpers.Constants.ResultMessages.NewRdoSuccess} {GetApiUsed()}"
                        });
                }
                catch (Exception)
                {
                    //Log Exception
                    return View("Confirmation",
                        new ResultModel
                        {
                            Header = Helpers.Constants.ViewHeaderText.NewRdo,
                            Message = $"{Helpers.Constants.ResultMessages.NewRdoFail} {GetApiUsed()}"
                        });
                }
            }
            else
            {
                //todo: return to the form with error message
                return View(newRdoModel);
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditRdo(String AppID, String ArtifactID)
        {
            if (AppID == null)
            {
                throw new ArgumentNullException(nameof(AppID));
            }

            if (ArtifactID == null)
            {
                throw new ArgumentNullException(nameof(ArtifactID));
            }

            int workspaceArtifactId;
            if (!int.TryParse(AppID, out workspaceArtifactId))
            {
                throw new ArgumentException($"{nameof(AppID)} is not an integer. [Value = {AppID}]");
            }

            int rdoArtifactId;
            if (!int.TryParse(ArtifactID, out rdoArtifactId))
            {
                throw new ArgumentException($"{nameof(ArtifactID)} is not an integer. [Value = {ArtifactID}]");
            }

            EditRdoModel editRdoModel = await ApiHelper.RetrieveCurrentValuesAsync(workspaceArtifactId, rdoArtifactId);
            return View(editRdoModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditRdo(EditRdoModel editRdoModel)
        {
            if (ModelState.IsValid)
            {
                if (editRdoModel == null)
                {
                    throw new ArgumentNullException(nameof(editRdoModel));
                }

                try
                {
                    await ApiHelper.UpdateNewValuesAsync(editRdoModel);
                    return View("Confirmation",
                        new ResultModel
                        {
                            Header = Helpers.Constants.ViewHeaderText.EditRdo,
                            Message = $"{Helpers.Constants.ResultMessages.EditRdoSuccess} {GetApiUsed()}"
                        });
                }
                catch (Exception)
                {
                    //Log Exception
                    return View("Confirmation",
                        new ResultModel
                        {
                            Header = Helpers.Constants.ViewHeaderText.EditRdo,
                            Message = $"{Helpers.Constants.ResultMessages.EditRdoFail} {GetApiUsed()}"
                        });
                }
            }
            else
            {
                //todo: return to the form with error message
                return View(editRdoModel);
            }
        }

        #region private methods

        private string GetApiUsed()
        {
            string apiUsed = string.Empty;
            if (ApiHelper is RsapiHelper)
            {
                apiUsed = "[RSAPI]";
            }
            if (ApiHelper is ObjectManagerHelper)
            {
                apiUsed = "[ObjectManager API]";
            }
            if (ApiHelper is GravityHelper)
            {
                apiUsed = "[Gravity API]";
            }

            return apiUsed;
        }

        #endregion
    }
}