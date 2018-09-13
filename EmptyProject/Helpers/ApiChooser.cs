using Helpers.DTOs;
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
			RDO jobRdo = null;
			
			jobRdo = RsapiHelper.RetrieveJob(servicesMgr, workspaceArtifactId, jobArtifactId);

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
			Choice metricChoice = null;

			metricChoice = RsapiHelper.RetrieveMetricChoice(servicesMgr, workspaceArtifactId, choiceArtifactId);
			
			return metricChoice;
		}

		public int QueryNumberOfWorkspaces(IServicesMgr servicesMgr)
		{
			int numberOfWorkspaces;
			
			numberOfWorkspaces = RsapiHelper.QueryNumberOfWorkspaces(servicesMgr);

			return numberOfWorkspaces;
		}

		public int QueryNumberOfUsers(IServicesMgr servicesMgr)
		{
			int numberOfUsers;

			numberOfUsers = RsapiHelper.QueryNumberOfUsers(servicesMgr);

			return numberOfUsers;
		}

		public int QueryNumberOfGroups(IServicesMgr servicesMgr)
		{
			int numberOfGroups;
			
			numberOfGroups = RsapiHelper.QueryNumberOfGroups(servicesMgr);

			return numberOfGroups;
		}
	}
}
