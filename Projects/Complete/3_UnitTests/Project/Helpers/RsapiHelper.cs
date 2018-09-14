using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Choice = kCura.Relativity.Client.DTOs.Choice;
using User = kCura.Relativity.Client.DTOs.User;

namespace Helpers
{
	public class RsapiHelper
	{
		public APIOptions RsapiApiOptions { get; set; }
		public IGenericRepository<RDO> RdoRepository { get; set; }
		public IGenericRepository<Choice> ChoiceRepository { get; set; }
		public IGenericRepository<Workspace> WorkspaceRepository { get; set; }
		public IGenericRepository<User> UserRepository { get; set; }
		public IGenericRepository<Group> GroupRepository { get; set; }

		public RsapiHelper(APIOptions rsapiApiOptions, IGenericRepository<RDO> rdoRepository, IGenericRepository<Choice> choiceRepository, IGenericRepository<Workspace> workspaceRepository, IGenericRepository<User> userRepository, IGenericRepository<Group> groupRepository)
		{
			RsapiApiOptions = rsapiApiOptions;
			RdoRepository = rdoRepository;
			ChoiceRepository = choiceRepository;
			WorkspaceRepository = workspaceRepository;
			UserRepository = userRepository;
			GroupRepository = groupRepository;
		}

		public List<int> RetrieveJobsInWorkspaceWithStatus(int workspaceArtifactId, string status)
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
				try
				{
					RsapiApiOptions.WorkspaceID = workspaceArtifactId;
					rdoQueryResultSet = RdoRepository.Query(rdoQuery);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_APPLICATION_JOBS_ERROR}. Query. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}]", ex);
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

		public RDO RetrieveJob(int workspaceArtifactId, int jobArtifactId)
		{
			RDO jobRdo;
			try
			{
				try
				{
					RsapiApiOptions.WorkspaceID = workspaceArtifactId;
					jobRdo = RdoRepository.ReadSingle(jobArtifactId);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.RETRIEVE_APPLICATION_JOB_ERROR}. ReadSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(jobArtifactId)}= {jobArtifactId}]", ex);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.RETRIEVE_APPLICATION_JOB_ERROR, ex);
			}
			return jobRdo;
		}

		public void UpdateJobField(int workspaceArtifactId, int jobArtifactId, Guid fieldGuid, object fieldValue)
		{
			try
			{
				RDO jobRdo = new RDO(jobArtifactId);
				jobRdo.ArtifactTypeGuids.Add(Constants.Guids.ObjectType.InstanceMetricsJob);
				jobRdo.Fields.Add(new FieldValue(fieldGuid, fieldValue));

				try
				{
					RsapiApiOptions.WorkspaceID = workspaceArtifactId;
					RdoRepository.UpdateSingle(jobRdo);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.UPDATE_APPLICATION_JOB_STATUS_ERROR}. UpdateSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(jobArtifactId)}= {jobArtifactId}, {nameof(fieldGuid)}= {fieldGuid}, {nameof(fieldValue)}= {fieldValue}]", ex);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.UPDATE_APPLICATION_JOB_STATUS_ERROR, ex);
			}
		}

		public Choice RetrieveMetricChoice(int workspaceArtifactId, int choiceArtifactId)
		{
			Choice metricChoice;
			try
			{
				try
				{
					RsapiApiOptions.WorkspaceID = workspaceArtifactId;
					metricChoice = ChoiceRepository.ReadSingle(choiceArtifactId);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.RETRIEVE_METRIC_CHOICE_ERROR}. ReadSingle. [{nameof(workspaceArtifactId)}= {workspaceArtifactId}, {nameof(choiceArtifactId)}= {choiceArtifactId}]", ex);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.RETRIEVE_METRIC_CHOICE_ERROR, ex);
			}
			return metricChoice;
		}

		public int QueryNumberOfWorkspaces()
		{
			int numberOfWorkspaces;
			try
			{
				Query<Workspace> workspaceQuery = new Query<Workspace>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<Workspace> workspaceQueryResultSet;
				try
				{
					RsapiApiOptions.WorkspaceID = -1;
					workspaceQueryResultSet = WorkspaceRepository.Query(workspaceQuery);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_WORKSPACES_ERROR}. Query.", ex);
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

		public int QueryNumberOfUsers()
		{
			int numberOfUsers;
			try
			{
				Query<User> userQuery = new Query<User>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<User> userQueryResultSet;
				try
				{
					RsapiApiOptions.WorkspaceID = -1;
					userQueryResultSet = UserRepository.Query(userQuery);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_USERS_ERROR}. Query.", ex);
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

		public int QueryNumberOfGroups()
		{
			int numberOfGroups;
			try
			{
				Query<Group> groupQuery = new Query<Group>
				{
					Fields = FieldValue.NoFields
				};

				QueryResultSet<Group> groupQueryResultSet;
				try
				{
					RsapiApiOptions.WorkspaceID = -1;
					groupQueryResultSet = GroupRepository.Query(groupQuery);
				}
				catch (Exception ex)
				{
					throw new Exception($"{Constants.ErrorMessages.QUERY_NUMBER_OF_GROUPS_ERROR}. Query.", ex);
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
