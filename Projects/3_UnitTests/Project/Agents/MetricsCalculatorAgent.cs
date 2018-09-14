using Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Choice = kCura.Relativity.Client.DTOs.Choice;
using Constants = Helpers.Constants;

namespace Agents
{
	[kCura.Agent.CustomAttributes.Name(Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR)]
	[System.Runtime.InteropServices.Guid("20530D16-D825-4FA5-9A7E-6760579EB07B")]
	public class MetricsCalculatorAgent : kCura.Agent.AgentBase
	{
		private IAPILog _logger;
		private RsapiHelper _rsapiHelper;

		public override string Name => Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR;

		public override void Execute()
		{
			RaiseMessage("Enter Agent", 10);

			try
			{
				_logger = Helper.GetLoggerFactory().GetLogger();
				IDBContext eddsDbContext = Helper.GetDBContext(-1);
				SetupRsapiHelper();

				RaiseMessage("Querying for workspaces where the application is installed.", 10);
				DataTable workspacesDataTable = SqlHelper.RetrieveApplicationWorkspaces(eddsDbContext, Constants.Guids.Application);

				if (workspacesDataTable == null || workspacesDataTable.Rows.Count <= 0)
				{
					return;
				}

				RaiseMessage($"Workspaces found({workspacesDataTable.Rows.Count})", 10);
				foreach (DataRow currentWorkspaceDataRow in workspacesDataTable.Rows)
				{
					int workspaceArtifactId = (int)currentWorkspaceDataRow["ArtifactID"];
					ProcessWorkspace(workspaceArtifactId);
				}

				RaiseMessage("Exit Agent", 10);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{Constants.Names.APPLICATION} - An error occured in the Agent.", ex);
				RaiseMessage("An error occured in the Agent. Check Error Logs.", 10);
			}
		}

		private void SetupRsapiHelper()
		{
			try
			{
				APIOptions rsapiApiOptions = new APIOptions
				{
					WorkspaceID = -1
				};

				IRSAPIClient rsapiClient = Helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
				rsapiClient.APIOptions = rsapiApiOptions;
				IGenericRepository<RDO> rdoRepository = rsapiClient.Repositories.RDO;
				IGenericRepository<Choice> choiceRepository = rsapiClient.Repositories.Choice;
				IGenericRepository<Workspace> workspaceRepository = rsapiClient.Repositories.Workspace;
				IGenericRepository<kCura.Relativity.Client.DTOs.User> userRepository = rsapiClient.Repositories.User;
				IGenericRepository<Group> groupRepository = rsapiClient.Repositories.Group;

				_rsapiHelper = new RsapiHelper(
					rsapiApiOptions: rsapiApiOptions,
					rdoRepository: rdoRepository,
					choiceRepository: choiceRepository,
					workspaceRepository: workspaceRepository,
					userRepository: userRepository,
					groupRepository: groupRepository);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.RSAPI_HELPER_SETUP_ERROR, ex);
			}
		}

		private void ProcessWorkspace(int workspaceArtifactId)
		{
			try
			{
				RaiseMessage($"Processing Workspace({workspaceArtifactId})", 10);
				RaiseMessage("Querying for jobs in the workspace", 10);
				List<int> newJobArtifactIds = _rsapiHelper.RetrieveJobsInWorkspaceWithStatus(workspaceArtifactId, Constants.JobStatus.NEW);
				foreach (int newJobArtifactId in newJobArtifactIds)
				{
					RaiseMessage($"Processing job({newJobArtifactId}) in the workspace({workspaceArtifactId})", 10);
					ProcessJob(workspaceArtifactId, newJobArtifactId);
					RaiseMessage($"Finished processing job({newJobArtifactId}) in the workspace({workspaceArtifactId})", 10);
				}
				RaiseMessage($"Finished processing Workspace({workspaceArtifactId})", 10);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_SINGLE_WORKSPACE_ERROR, ex);
			}
		}

		private void ProcessJob(int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				//Update job status to In Progress
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.IN_PROGRESS);

				//Update job metrics
				RDO jobRdo = _rsapiHelper.RetrieveJob(workspaceArtifactId, jobArtifactId);
				RaiseMessage("Calculating metrics for the job", 10);
				ProcessAllMetrics(workspaceArtifactId, jobArtifactId, jobRdo);
				RaiseMessage("Calculated metrics for the job", 10);

				//Update job status to Completed
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.COMPLETED);
			}
			catch (Exception ex)
			{
				//Update job status to Error
				string errorMessage = ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex);
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.ERROR);
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Errors_LongText, errorMessage);
			}
		}

		private void ProcessAllMetrics(int workspaceArtifactId, int jobArtifactId, RDO jobRdo)
		{
			try
			{
				MultiChoiceFieldValueList metricMultipleChoices = jobRdo.Fields.Get(Constants.Guids.Fields.InstanceMetricsJob.Metrics_MultipleChoice).ValueAsMultipleChoice;
				List<int> metricArtifactIdsToCollect = metricMultipleChoices.Select(x => x.ArtifactID).ToList();
				List<Guid> metricGuidsToCollect = ConvertChoiceArtifactIdsToGuids(workspaceArtifactId, metricArtifactIdsToCollect);

				foreach (Guid metricGuid in metricGuidsToCollect)
				{
					ProcessSingleMetric(workspaceArtifactId, jobArtifactId, metricGuid);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_ALL_JOB_METRICS_ERROR, ex);
			}
		}

		private List<Guid> ConvertChoiceArtifactIdsToGuids(int workspaceArtifactId, List<int> choiceArtifactIds)
		{
			try
			{
				List<Guid> choiceGuids = new List<Guid>();
				foreach (int choiceArtifactId in choiceArtifactIds)
				{
					Choice choice = _rsapiHelper.RetrieveMetricChoice(workspaceArtifactId, choiceArtifactId);
					Guid choiceGuid = choice.Guids.FirstOrDefault();
					choiceGuids.Add(choiceGuid);
				}
				return choiceGuids;
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.CHOICE_ARTIFACT_ID_TO_GUID_CONVERSION_ERROR, ex);
			}
		}

		private void ProcessSingleMetric(int workspaceArtifactId, int jobArtifactId, Guid metricGuid)
		{
			try
			{
				if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Workspaces))
				{
					ProcessWorkspacesMetric(workspaceArtifactId, jobArtifactId);
				}
				else if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Users))
				{
					ProcessUsersMetric(workspaceArtifactId, jobArtifactId);
				}
				else if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Groups))
				{
					ProcessGroupsMetric(workspaceArtifactId, jobArtifactId);
				}
				else
				{
					throw new Exception(Constants.ErrorMessages.INVALID_METRIC_ERROR);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_SINGLE_JOB_METRIC_ERROR, ex);
			}
		}

		private void ProcessWorkspacesMetric(int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing workspaces metric", 10);
				int noOfWorkspaces = _rsapiHelper.QueryNumberOfWorkspaces();
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.WorkspacesCount_LongText, noOfWorkspaces);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_WORKSPACES_METRIC_ERROR, ex);
			}
		}

		private void ProcessUsersMetric(int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing users metric", 10);
				int noOfUsers = _rsapiHelper.QueryNumberOfUsers();
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.UsersCount_LongText, noOfUsers);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_USERS_METRIC_ERROR, ex);
			}
		}

		private void ProcessGroupsMetric(int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing groups metric", 10);
				int noOfGroups = _rsapiHelper.QueryNumberOfGroups();
				_rsapiHelper.UpdateJobField(workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.GroupsCount_LongText, noOfGroups);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_GROUPS_METRIC_ERROR, ex);
			}
		}
	}
}
