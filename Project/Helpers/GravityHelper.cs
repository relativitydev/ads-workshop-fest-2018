using Gravity.DAL.RSAPI;
using Helpers.DTOs;
using kCura.Relativity.Client;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Helpers
{
	public class GravityHelper
	{
		public static List<int> RetrieveJobsInWorkspaceWithStatus(IServicesMgr servicesMgr, int workspaceArtifactId, string status)
		{
			RsapiDao rsapiDao = new RsapiDao(servicesMgr, workspaceArtifactId, ExecutionIdentity.System);
			
			Guid fieldGuid = typeof(InstanceMetricsJobObj).GetProperty(nameof(InstanceMetricsJobObj.Status)).GetCustomAttribute<RelativityObjectFieldAttribute>().FieldGuid;

			Condition condition = new TextCondition(fieldGuid, TextConditionEnum.EqualTo, status);

			List<int> jobsList = rsapiDao.Query<InstanceMetricsJobObj>(condition, Gravity.Base.ObjectFieldsDepthLevel.FirstLevelOnly).Select(x => x.ArtifactId).ToList();

			return jobsList;
		}

		public static InstanceMetricsJobObj RetrieveJob(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			RsapiDao rsapiDao = new RsapiDao(servicesMgr, workspaceArtifactId, ExecutionIdentity.System);
			InstanceMetricsJobObj jobRdo = new InstanceMetricsJobObj();

			jobRdo = rsapiDao.Get<InstanceMetricsJobObj>(jobArtifactId, Gravity.Base.ObjectFieldsDepthLevel.OnlyParentObject);
			return jobRdo;
		}

		public static void UpdateJobField(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{
			RsapiDao rsapiDao = new RsapiDao(servicesMgr, workspaceArtifactId, ExecutionIdentity.System);
			rsapiDao.UpdateField<InstanceMetricsJobObj>(jobArtifactId, fieldGuid, fieldValue);
		}
	}
}
