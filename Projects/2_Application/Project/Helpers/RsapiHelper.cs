using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using User = kCura.Relativity.Client.DTOs.User;

namespace Helpers
{
	public class RsapiHelper
	{
		public static List<int> RetrieveJobsInWorkspaceWithStatus(IServicesMgr servicesMgr, int workspaceArtifactId, string status)
		{
			List<int> jobsList = new List<int>();
			try
			{
				Query<RDO> rdoQuery = new Query<RDO>
				{
					ArtifactTypeGuid = Constants.Guids.ObjectType.InstanceMetricsJob,
					Fields = FieldValue.NoFields,
					Condition = new TextCondition(Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, TextConditionEnum.EqualTo, status)
				};

				QueryResultSet<RDO> rdoQueryResultSet;
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
					try
					{
						rdoQueryResultSet = rsapiClient.Repositories.RDO.Query(rdoQuery);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.QUERY_APPLICATION_JOBS_ERROR}. Query. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}]", ex);
					}
				}

				if (!rdoQueryResultSet.Success)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_APPLICATION_JOBS_ERROR}. Query. Error Message: {rdoQueryResultSet.Message}. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}]");
				}

				jobsList.AddRange(rdoQueryResultSet.Results.Select(x => x.Artifact.ArtifactID));
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.QUERY_APPLICATION_JOBS_ERROR, ex);
			}
			return jobsList;
		}

		public static RDO RetrieveJob(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			RDO jobRdo;
			try
			{
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
					try
					{
						jobRdo = rsapiClient.Repositories.RDO.ReadSingle(jobArtifactId);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.RETRIEVE_APPLICATION_JOB_ERROR}. ReadSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(jobArtifactId)}= {jobArtifactId}]", ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.RETRIEVE_APPLICATION_JOB_ERROR, ex);
			}
			return jobRdo;
		}

		public static void UpdateJobField(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{
			try
			{
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO jobRdo = new RDO(jobArtifactId);
					jobRdo.ArtifactTypeGuids.Add(Constants.Guids.ObjectType.InstanceMetricsJob);
					jobRdo.Fields.Add(new FieldValue(fieldGuid, fieldValue));

					try
					{
						rsapiClient.Repositories.RDO.UpdateSingle(jobRdo);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.UPDATE_APPLICATION_JOB_STATUS_ERROR}. UpdateSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(jobArtifactId)}= {jobArtifactId}, {nameof(fieldGuid)}= {fieldGuid}, {nameof(fieldValue)}= {fieldValue}]", ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.UPDATE_APPLICATION_JOB_STATUS_ERROR, ex);
			}
		}

		public static kCura.Relativity.Client.DTOs.Choice RetrieveMetricChoice(IServicesMgr servicesMgr, int workspaceArtifactId, int choiceArtifactId)
		{
			kCura.Relativity.Client.DTOs.Choice metricChoice;
			try
			{
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
					try
					{
						metricChoice = rsapiClient.Repositories.Choice.ReadSingle(choiceArtifactId);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.RETRIEVE_METRIC_CHOICE_ERROR}. ReadSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(choiceArtifactId)}= {choiceArtifactId}]", ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.RETRIEVE_METRIC_CHOICE_ERROR, ex);
			}
			return metricChoice;
		}

		public static int QueryNumberOfWorkspaces(IServicesMgr servicesMgr)
		{
			int numberOfWorkspaces;
			try
			{
				Query<Workspace> workspaceQuery = new Query<Workspace>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<Workspace> workspaceQueryResultSet;
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = -1;
					try
					{
						workspaceQueryResultSet = rsapiClient.Repositories.Workspace.Query(workspaceQuery);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_WORKSPACES_ERROR}. Query.", ex);
					}
				}

				if (!workspaceQueryResultSet.Success)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_WORKSPACES_ERROR}. Query. Error Message: {workspaceQueryResultSet.Message}.");
				}

				numberOfWorkspaces = workspaceQueryResultSet.Results.Count;
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.QUERY_NUMBER_OF_WORKSPACES_ERROR, ex);
			}
			return numberOfWorkspaces;
		}

		public static int QueryNumberOfUsers(IServicesMgr servicesMgr)
		{
			int numberOfUsers;
			try
			{
				Query<User> userQuery = new Query<User>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<User> userQueryResultSet;
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = -1;
					try
					{
						userQueryResultSet = rsapiClient.Repositories.User.Query(userQuery);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_USERS_ERROR}. Query.", ex);
					}
				}

				if (!userQueryResultSet.Success)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_USERS_ERROR}. Query. Error Message: {userQueryResultSet.Message}.");
				}

				numberOfUsers = userQueryResultSet.Results.Count;
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.QUERY_NUMBER_OF_USERS_ERROR, ex);
			}
			return numberOfUsers;
		}

		public static int QueryNumberOfGroups(IServicesMgr servicesMgr)
		{
			int numberOfGroups;
			try
			{
				Query<Group> groupQuery = new Query<Group>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<Group> groupQueryResultSet;
				using (IRSAPIClient rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = -1;
					try
					{
						groupQueryResultSet = rsapiClient.Repositories.Group.Query(groupQuery);
					}
					catch (Exception ex)
					{
						throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_GROUPS_ERROR}. Query.", ex);
					}
				}

				if (!groupQueryResultSet.Success)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_GROUPS_ERROR}. Query. Error Message: {groupQueryResultSet.Message}.");
				}

				numberOfGroups = groupQueryResultSet.Results.Count;
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.QUERY_NUMBER_OF_GROUPS_ERROR, ex);
			}
			return numberOfGroups;
		}
	}
}
