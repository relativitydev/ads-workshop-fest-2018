using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using OverrideCustomPage.Exceptions;
using OverrideCustomPage.Models;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable AccessToDisposedClosure

namespace OverrideCustomPage.Helpers
{
    public class RsapiHelper : IApiHelper
    {
        public IAPILog ApiLog { get; set; }
        private IGenericRepository<RDO> RdoRepository { get; set; }
        private APIOptions RsapiApiOptions { get; set; }

        public RsapiHelper(IAPILog apiLog, IGenericRepository<RDO> rdoRepository, APIOptions rsapiApiOptions)
        {
            ApiLog = apiLog ?? throw new ArgumentNullException(nameof(apiLog));
            RdoRepository = rdoRepository ?? throw new ArgumentNullException(nameof(rdoRepository));
            RsapiApiOptions = rsapiApiOptions ?? throw new ArgumentNullException(nameof(rsapiApiOptions));
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
                RsapiApiOptions.WorkspaceID = newRdoModel.WorkspaceArtifactId;
                RDO rdo = new RDO
                {
                    ArtifactTypeGuids = new List<Guid> { Constants.Guids.ObjectTypes.OverrideRdo },
                    Fields = new List<FieldValue>
                    {
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Name, newRdoModel.Name),
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Phone, newRdoModel.Phone),
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Email, newRdoModel.Email)
                    }
                };

                try
                {
                    await Task.Run(() => { RdoRepository.CreateSingle(rdo); });
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.NewRdoCreateError}. CreateSingle.";
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
                RsapiApiOptions.WorkspaceID = workspaceArtifactId;
                RDO rdo;
                try
                {
                    rdo = await Task.Run(() => RdoRepository.ReadSingle(rdoArtifactId));
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.RdoReadError}. ReadSingle.";
                    ApiLog.LogError(errorMessage, ex);
                    throw new OverrideCustomPageException(errorMessage, ex);
                }

                EditRdoModel editRdoModel = new EditRdoModel
                {
                    WorkspaceArtifactId = workspaceArtifactId,
                    RdoArtifactId = rdoArtifactId,
                    Name = rdo.Fields.Get(Constants.Guids.Fields.OverrideRdo.Name).ValueAsFixedLengthText,
                    Phone = rdo.Fields.Get(Constants.Guids.Fields.OverrideRdo.Phone).ValueAsFixedLengthText,
                    Email = rdo.Fields.Get(Constants.Guids.Fields.OverrideRdo.Email).ValueAsFixedLengthText
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
                RsapiApiOptions.WorkspaceID = editRdoModel.WorkspaceArtifactId;
                RDO rdo = new RDO(editRdoModel.RdoArtifactId)
                {
                    ArtifactTypeGuids = new List<Guid> { Constants.Guids.ObjectTypes.OverrideRdo },
                    Fields = new List<FieldValue>
                    {
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Name,editRdoModel.Name),
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Phone,editRdoModel.Phone),
                        new FieldValue(Constants.Guids.Fields.OverrideRdo.Email,editRdoModel.Email)
                    }
                };

                try
                {
                    await Task.Run(() => RdoRepository.UpdateSingle(rdo));
                }
                catch (Exception ex)
                {
                    //Log exception
                    string errorMessage = $"{Constants.ErrorMessages.EditRdoSaveError}. UpdateSingle.";
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
}