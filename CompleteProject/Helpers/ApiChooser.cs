using Helpers.DTOs;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Collections.Generic;
using Choice = kCura.Relativity.Client.DTOs.Choice;

namespace Helpers
{
	public class ApiChooser
	{
		private readonly Constants.ApiType _apiType;
		private readonly APIOptions _rsapiApiOptions;

		public ApiChooser(Constants.ApiType apiType)
		{
			_apiType = apiType;
			_rsapiApiOptions = new APIOptions
			{
				WorkspaceID = -1
			};
		}

		public List<int> RetrieveJobsInWorkspaceWithStatus(IServicesMgr servicesMgr, int workspaceArtifactId, string status)
		{
			List<int> jobsList;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					RsapiHelper rsapiHelper = new RsapiHelper
					{
						RsapiApiOptions = _rsapiApiOptions,
						RdoRepository = rsapiClient.Repositories.RDO
					};
					jobsList = rsapiHelper.RetrieveJobsInWorkspaceWithStatus(workspaceArtifactId, status);
				}
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				jobsList = GravityHelper.RetrieveJobsInWorkspaceWithStatus(servicesMgr, workspaceArtifactId, status);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return jobsList;
		}

		public RDO RetrieveJob(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			RDO jobRdo = null;

			using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				RsapiHelper rsapiHelper = new RsapiHelper
				{
					RsapiApiOptions = _rsapiApiOptions,
					RdoRepository = rsapiClient.Repositories.RDO
				};
				jobRdo = rsapiHelper.RetrieveJob(workspaceArtifactId, jobArtifactId);
			}

			return jobRdo;
		}

		public InstanceMetricsJobObj RetrieveJobWithGravity(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			InstanceMetricsJobObj jobRdo = null;

			jobRdo = GravityHelper.RetrieveJob(servicesMgr, workspaceArtifactId, jobArtifactId);

			return jobRdo;
		}

		public void UpdateJobField(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{
			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					RsapiHelper rsapiHelper = new RsapiHelper
					{
						RsapiApiOptions = _rsapiApiOptions,
						RdoRepository = rsapiClient.Repositories.RDO
					};
					rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, fieldGuid, fieldValue);
				}
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				GravityHelper.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, fieldGuid, fieldValue);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}
		}

		public Choice RetrieveMetricChoice(IServicesMgr servicesMgr, int workspaceArtifactId, int choiceArtifactId)
		{
			Choice metricChoice = null;

			using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				RsapiHelper rsapiHelper = new RsapiHelper
				{
					RsapiApiOptions = _rsapiApiOptions,
					ChoiceRepository = rsapiClient.Repositories.Choice
				};
				metricChoice = rsapiHelper.RetrieveMetricChoice(workspaceArtifactId, choiceArtifactId);
			}

			return metricChoice;
		}

		public int QueryNumberOfWorkspaces(IServicesMgr servicesMgr)
		{
			int numberOfWorkspaces;

			using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				RsapiHelper rsapiHelper = new RsapiHelper
				{
					RsapiApiOptions = _rsapiApiOptions,
					WorkspaceRepository = rsapiClient.Repositories.Workspace
				};
				numberOfWorkspaces = rsapiHelper.QueryNumberOfWorkspaces();
			}

			return numberOfWorkspaces;
		}

		public int QueryNumberOfUsers(IServicesMgr servicesMgr)
		{
			int numberOfUsers;

			using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				RsapiHelper rsapiHelper = new RsapiHelper
				{
					RsapiApiOptions = _rsapiApiOptions,
					UserRepository = rsapiClient.Repositories.User
				};
				numberOfUsers = rsapiHelper.QueryNumberOfUsers();
			}

			return numberOfUsers;
		}

		public int QueryNumberOfGroups(IServicesMgr servicesMgr)
		{
			int numberOfGroups;

			using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				RsapiHelper rsapiHelper = new RsapiHelper
				{
					RsapiApiOptions = _rsapiApiOptions,
					GroupRepository = rsapiClient.Repositories.Group
				};
				numberOfGroups = rsapiHelper.QueryNumberOfGroups();
			}

			return numberOfGroups;
		}
	}
}
