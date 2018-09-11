using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace Helpers
{
	public class GravityHelper
	{
		public static List<int> RetrieveJobsInWorkspaceWithStatus(IServicesMgr servicesMgr, int workspaceArtifactId, string status)
		{
			List<int> jobsList = new List<int>();
			return jobsList;
		}

		public static RDO RetrieveJob(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			RDO jobRdo = new RDO(123);
			return jobRdo;
		}

		public static void UpdateJobField(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{

		}

		public static Choice RetrieveMetricChoice(IServicesMgr servicesMgr, int workspaceArtifactId, int choiceArtifactId)
		{
			Choice metricChoice = new Choice();
			return metricChoice;
		}

		public static int QueryNumberOfWorkspaces(IServicesMgr servicesMgr)
		{
			int numberOfWorkspaces = -1;
			return numberOfWorkspaces;
		}

		public static int QueryNumberOfUsers(IServicesMgr servicesMgr)
		{
			int numberOfUsers = -1;
			return numberOfUsers;
		}

		public static int QueryNumberOfGroups(IServicesMgr servicesMgr)
		{
			int numberOfGroups = -1;
			return numberOfGroups;
		}
	}
}
