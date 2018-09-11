using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace Helpers
{
	public class ApiChooser
	{
		private readonly Constants.ApiType _apiType;

		public ApiChooser(Constants.ApiType apiType)
		{
			_apiType = apiType;
		}

		public List<int> RetrieveJobsInWorkspaceWithStatus(IServicesMgr servicesMgr, int workspaceArtifactId, string status)
		{
			List<int> jobsList;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				jobsList = RsapiHelper.RetrieveJobsInWorkspaceWithStatus(servicesMgr, workspaceArtifactId, status);
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
			RDO jobRdo;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				jobRdo = RsapiHelper.RetrieveJob(servicesMgr, workspaceArtifactId, jobArtifactId);
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				jobRdo = GravityHelper.RetrieveJob(servicesMgr, workspaceArtifactId, jobArtifactId);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return jobRdo;
		}

		public void UpdateJobField(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{
			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				RsapiHelper.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, fieldGuid, fieldValue);
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
			Choice metricChoice;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				metricChoice = RsapiHelper.RetrieveMetricChoice(servicesMgr, workspaceArtifactId, choiceArtifactId);
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				metricChoice = GravityHelper.RetrieveMetricChoice(servicesMgr, workspaceArtifactId, choiceArtifactId);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return metricChoice;
		}

		public int QueryNumberOfWorkspaces(IServicesMgr servicesMgr)
		{
			int numberOfWorkspaces;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				numberOfWorkspaces = RsapiHelper.QueryNumberOfWorkspaces(servicesMgr);
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				numberOfWorkspaces = GravityHelper.QueryNumberOfWorkspaces(servicesMgr);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return numberOfWorkspaces;
		}

		public int QueryNumberOfUsers(IServicesMgr servicesMgr)
		{
			int numberOfUsers;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				numberOfUsers = RsapiHelper.QueryNumberOfUsers(servicesMgr);
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				numberOfUsers = GravityHelper.QueryNumberOfUsers(servicesMgr);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return numberOfUsers;
		}

		public int QueryNumberOfGroups(IServicesMgr servicesMgr)
		{
			int numberOfGroups;

			if (_apiType.Equals(Constants.ApiType.Rsapi))
			{
				numberOfGroups = RsapiHelper.QueryNumberOfGroups(servicesMgr);
			}
			else if (_apiType.Equals(Constants.ApiType.Gravity))
			{
				numberOfGroups = GravityHelper.QueryNumberOfGroups(servicesMgr);
			}
			else
			{
				throw new Exception(Constants.ErrorMessages.INVALID_API_TYPE_ERROR);
			}

			return numberOfGroups;
		}
	}
}
