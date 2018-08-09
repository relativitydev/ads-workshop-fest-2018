using OverrideCustomPage.Exceptions;
using OverrideCustomPage.Models;
using Relativity.API;
using Relativity.Services.Field;
using Relativity.Services.Objects;
using Relativity.Services.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverrideCustomPage.Helpers
{
    public class ObjectManagerHelper : IApiHelper
    {
        public IAPILog ApiLog { get; set; }
        public IServicesMgr ServicesMgr { get; set; }

        public ObjectManagerHelper(IAPILog apiLog, IServicesMgr servicesMgr)
        {
            ApiLog = apiLog ?? throw new ArgumentNullException(nameof(apiLog));
            ServicesMgr = servicesMgr ?? throw new ArgumentNullException(nameof(servicesMgr));
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

            //Create functionality not available using ObjectManager API. Will be released in future versions.
            await Task.Run(() => { });
            throw new NotImplementedException();
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
                using (IObjectManager objectManager = ServicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.System))
                {
                    ObjectReadResult objectReadResult;

                    IList<FieldRef> fieldRefs = new List<FieldRef>
                    {
                        new FieldRef()
                        {
                            Guids = new List<Guid>
                            {
                                Constants.Guids.Fields.OverrideRdo.Name
                            }
                        },
                        new FieldRef()
                        {
                            Guids = new List<Guid>
                            {
                                Constants.Guids.Fields.OverrideRdo.Phone
                            }
                        },
                        new FieldRef()
                        {
                            Guids = new List<Guid>
                            {
                                Constants.Guids.Fields.OverrideRdo.Email
                            }
                        }
                    };
                    try
                    {
                        objectReadResult = await objectManager.ReadSingleAsync(workspaceArtifactId, rdoArtifactId, fieldRefs);
                    }
                    catch (Exception ex)
                    {
                        //Log exception
                        string errorMessage = $"{Constants.ErrorMessages.RdoReadError}. ReadSingleAsync.";
                        ApiLog.LogError(errorMessage, ex);
                        throw new OverrideCustomPageException(errorMessage, ex);
                    }

                    EditRdoModel editRdoModel = new EditRdoModel
                    {
                        WorkspaceArtifactId = workspaceArtifactId,
                        RdoArtifactId = rdoArtifactId,
                        Name = objectReadResult.RelativityObject.FieldValuePairs.First(x => x.Field.Name.Equals(Constants.FieldNames.OverrideRdo.Name)).Value.ToString(),
                        Phone = objectReadResult.RelativityObject.FieldValuePairs.First(x => x.Field.Name.Equals(Constants.FieldNames.OverrideRdo.Phone)).Value.ToString(),
                        Email = objectReadResult.RelativityObject.FieldValuePairs.First(x => x.Field.Name.Equals(Constants.FieldNames.OverrideRdo.Email)).Value.ToString()
                    };

                    return editRdoModel;
                }
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
                using (IObjectManager objectManager = ServicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.System))
                {
                    RelativityObject relativityObject = new RelativityObject
                    {
                        ArtifactID = editRdoModel.RdoArtifactId,
                        FieldValuePairs = new List<FieldValuePair>
                        {
                            new FieldValuePair
                            {
                                Field = new FieldRef()
                                {
                                    Guids = new List<Guid>
                                    {
                                        Constants.Guids.Fields.OverrideRdo.Name
                                    }
                                },
                                Value = editRdoModel.Name
                            },
                            new FieldValuePair
                            {
                                Field = new FieldRef()
                                {
                                    Guids = new List<Guid>
                                    {
                                        Constants.Guids.Fields.OverrideRdo.Phone
                                    }
                                },
                                Value = editRdoModel.Phone
                            },
                            new FieldValuePair
                            {
                                Field = new FieldRef()
                                {
                                    Guids = new List<Guid>
                                    {
                                        Constants.Guids.Fields.OverrideRdo.Email
                                    }
                                },
                                Value = editRdoModel.Email
                            }
                        }
                    };

                    try
                    {
                        await objectManager.UpdateAsync(editRdoModel.WorkspaceArtifactId, editRdoModel.RdoArtifactId, relativityObject);
                    }
                    catch (Exception ex)
                    {
                        //Log exception
                        string errorMessage = $"{Constants.ErrorMessages.EditRdoSaveError}. UpdateAsync.";
                        ApiLog.LogError(errorMessage, ex);
                        throw new OverrideCustomPageException(errorMessage, ex);
                    }
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