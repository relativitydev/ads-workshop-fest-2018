using Gravity.Base;
using Gravity.DAL.RSAPI;
using OverrideCustomPage.Exceptions;
using OverrideCustomPage.Models;
using Relativity.API;
using System;
using System.Threading.Tasks;

namespace OverrideCustomPage.Helpers
{
    public class GravityHelper : IApiHelper
    {
        public IAPILog ApiLog { get; set; }
        public ICPHelper CpHelper { get; set; }

        public GravityHelper(IAPILog apiLog, ICPHelper cpHelper)
        {
            ApiLog = apiLog ?? throw new ArgumentNullException(nameof(apiLog));
            CpHelper = cpHelper ?? throw new ArgumentNullException(nameof(cpHelper));
        }

        public async Task SaveNewValuesAsync(NewRdoModel newRdoModel)
        {
            if (newRdoModel == null)
            {
                throw new ArgumentNullException(nameof(newRdoModel));
            }
            if (newRdoModel.WorkspaceArtifactId < 1)
            {
                throw new ArgumentException($"{nameof(newRdoModel.WorkspaceArtifactId)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(newRdoModel.Name))
            {
                throw new ArgumentException($"{nameof(newRdoModel.Name)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(newRdoModel.Phone))
            {
                throw new ArgumentException($"{nameof(newRdoModel.Phone)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(newRdoModel.Email))
            {
                throw new ArgumentException($"{nameof(newRdoModel.Email)} is not valid.");
            }

            try
            {
                RsapiDao gravityRsapiDao = new RsapiDao(CpHelper, newRdoModel.WorkspaceArtifactId);
                OverrideRdo newOverrideRdo = new OverrideRdo
                {
                    Name = newRdoModel.Name,
                    Phone = newRdoModel.Phone,
                    Email = newRdoModel.Email
                };

                try
                {
                    await Task.Run(() => { gravityRsapiDao.InsertRelativityObject<OverrideRdo>(newOverrideRdo); });
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.NewRdoCreateError}. InsertRelativityObject.";
                    ApiLog.LogError(errorMessage, ex);
                    throw new OverrideCustomPageException(errorMessage, ex);
                }
            }
            catch (Exception ex)
            {
                //Log exception
                string errorMessage = Constants.ErrorMessages.NewRdoCreateError;
                ApiLog.LogError(errorMessage, ex);
                throw new OverrideCustomPageException(errorMessage, ex);
            }
        }

        public async Task<EditRdoModel> RetrieveCurrentValuesAsync(int workspaceArtifactId, int rdoArtifactId)
        {
            if (workspaceArtifactId < 1)
            {
                throw new ArgumentException($"{nameof(workspaceArtifactId)} is not valid.");
            }
            if (rdoArtifactId < 1)
            {
                throw new ArgumentException($"{nameof(rdoArtifactId)} is not valid.");
            }

            try
            {
                RsapiDao gravityRsapiDao = new RsapiDao(CpHelper, workspaceArtifactId);
                OverrideRdo currentOverrideRdo;
                try
                {
                    currentOverrideRdo = await Task.Run(() => gravityRsapiDao.GetRelativityObject<OverrideRdo>(rdoArtifactId, ObjectFieldsDepthLevel.FirstLevelOnly));
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.RdoReadError}. GetRelativityObject.";
                    ApiLog.LogError(errorMessage, ex);
                    throw new OverrideCustomPageException(errorMessage, ex);
                }

                EditRdoModel editRdoModel = new EditRdoModel
                {
                    WorkspaceArtifactId = workspaceArtifactId,
                    RdoArtifactId = rdoArtifactId,
                    Name = currentOverrideRdo.Name,
                    Phone = currentOverrideRdo.Phone,
                    Email = currentOverrideRdo.Email
                };

                return editRdoModel;
            }
            catch (Exception ex)
            {
                //Log exception
                string errorMessage = Constants.ErrorMessages.RdoReadError;
                ApiLog.LogError(errorMessage, ex);
                throw new OverrideCustomPageException(errorMessage, ex);
            }
        }

        public async Task UpdateNewValuesAsync(EditRdoModel editRdoModel)
        {
            if (editRdoModel == null)
            {
                throw new ArgumentNullException(nameof(editRdoModel));
            }
            if (editRdoModel.WorkspaceArtifactId < 1)
            {
                throw new ArgumentException($"{nameof(editRdoModel.WorkspaceArtifactId)} is not valid.");
            }
            if (editRdoModel.RdoArtifactId < 1)
            {
                throw new ArgumentException($"{nameof(editRdoModel.RdoArtifactId)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(editRdoModel.Name))
            {
                throw new ArgumentException($"{nameof(editRdoModel.Name)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(editRdoModel.Phone))
            {
                throw new ArgumentException($"{nameof(editRdoModel.Phone)} is not valid.");
            }
            if (string.IsNullOrWhiteSpace(editRdoModel.Email))
            {
                throw new ArgumentException($"{nameof(editRdoModel.Email)} is not valid.");
            }

            try
            {
                RsapiDao gravityRsapiDao = new RsapiDao(CpHelper, editRdoModel.WorkspaceArtifactId);
                OverrideRdo editOverrideRdo = new OverrideRdo
                {
                    ArtifactId = editRdoModel.RdoArtifactId,
                    Name = editRdoModel.Name,
                    Phone = editRdoModel.Phone,
                    Email = editRdoModel.Email
                };

                try
                {
                    await Task.Run(() => gravityRsapiDao.UpdateRelativityObject<OverrideRdo>(editOverrideRdo));
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.EditRdoSaveError}. UpdateRelativityObject.";
                    ApiLog.LogError(errorMessage, ex);
                    throw new OverrideCustomPageException(errorMessage, ex);
                }
            }
            catch (Exception ex)
            {
                //Log exception
                string errorMessage = Constants.ErrorMessages.EditRdoSaveError;
                ApiLog.LogError(errorMessage, ex);
                throw new OverrideCustomPageException(errorMessage, ex);
            }
        }
    }

    [Serializable]
    [RelativityObject("A0B5E207-6F95-4CE8-A18A-D323DE09BE79")]
    public class OverrideRdo : BaseDto
    {
        [RelativityObjectField("614215D8-19C4-41AA-97F0-68BC09F8202C", (int)RdoFieldType.FixedLengthText, 255)]
        public override string Name { get; set; }

        [RelativityObjectField("50BBF393-E4E8-4BD9-AE8B-D7FCC450AAAC", (int)RdoFieldType.FixedLengthText, 255)]
        public string Phone { get; set; }

        [RelativityObjectField("7C38D7AD-4B39-4DED-A7AD-AB7D38A79B05", (int)RdoFieldType.FixedLengthText, 255)]
        public string Email { get; set; }
    }
}