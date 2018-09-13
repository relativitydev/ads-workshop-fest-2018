using Helpers;
using Helpers.DTOs;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Constants = Helpers.Constants;
using Gravity.Extensions;

namespace Agents
{
	[kCura.Agent.CustomAttributes.Name(Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR)]
	[System.Runtime.InteropServices.Guid("20530D16-D825-4FA5-9A7E-6760579EB07B")]
	public class MetricsCalculatorAgent : kCura.Agent.AgentBase
	{
		private IAPILog _logger;

		public override string Name => Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR;
		private static readonly Constants.ApiType selectedApiType = Constants.ApiType.Gravity;
		private readonly ApiChooser _apiChooser = new ApiChooser(selectedApiType);

		public override void Execute()
		{
			RaiseMessage("Enter Agent", 10);

			try
			{
				_logger = Helper.GetLoggerFactory().GetLogger();
				IServicesMgr servicesMgr = Helper.GetServicesManager();
				IDBContext eddsDbContext = Helper.GetDBContext(-1);

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
					ProcessWorkspace(servicesMgr, workspaceArtifactId);
				}

				//loop, get all jobs in the workspace with status New
				//process job
				//update job status
				RaiseMessage("Exit Agent", 10);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{Constants.Names.APPLICATION} - An error occured in the Agent.", ex);
				RaiseMessage("An error occured in the Agent. Check Error Logs.", 10);
			}
		}

		private void ProcessWorkspace(IServicesMgr servicesMgr, int workspaceArtifactId)
		{
			try
			{
				RaiseMessage($"Processing Workspace({workspaceArtifactId})", 10);
				RaiseMessage("Querying for jobs in the workspace", 10);
				List<int> newJobArtifactIds = _apiChooser.RetrieveJobsInWorkspaceWithStatus(servicesMgr, workspaceArtifactId, Constants.JobStatus.NEW);
				foreach (int newJobArtifactId in newJobArtifactIds)
				{
					RaiseMessage($"Processing job({newJobArtifactId}) in the workspace({workspaceArtifactId})", 10);
					ProcessJob(servicesMgr, workspaceArtifactId, newJobArtifactId);
					RaiseMessage($"Finished processing job({newJobArtifactId}) in the workspace({workspaceArtifactId})", 10);
				}
				RaiseMessage($"Finished processing Workspace({workspaceArtifactId})", 10);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_SINGLE_WORKSPACE_ERROR, ex);
			}
		}

		private void ProcessJob(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				//Update job status to In Progress
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.IN_PROGRESS);

				//Update job metrics
				if(selectedApiType == Helpers.Constants.ApiType.Rsapi)
				{
					RDO jobRdo = _apiChooser.RetrieveJob(servicesMgr, workspaceArtifactId, jobArtifactId);
					RaiseMessage("Calculating metrics for the job", 10);
					ProcessAllMetrics(servicesMgr, workspaceArtifactId, jobArtifactId, jobRdo);
					RaiseMessage("Calculated metrics for the job", 10);
				}
				else if(selectedApiType == Helpers.Constants.ApiType.Gravity)
				{
					InstanceMetricsJobObj jobRdo = _apiChooser.RetrieveJobWithGravity(servicesMgr, workspaceArtifactId, jobArtifactId);
					RaiseMessage("Calculating metrics for the job", 10);
					ProcessAllMetrics(servicesMgr, workspaceArtifactId, jobArtifactId, jobRdo);
					RaiseMessage("Calculated metrics for the job", 10);
				}

				//Update job status to Completed
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.COMPLETED);
			}
			catch (Exception ex)
			{
				//Update job status to Error
				string errorMessage = ExceptionMessageFormatter.GetInnerMostExceptionMessage(ex);
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, Constants.JobStatus.COMPLETED);
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.Errors_LongText, errorMessage);
			}
		}

		private void ProcessAllMetrics(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, RDO jobRdo)
		{
			try
			{
				MultiChoiceFieldValueList metricMultipleChoices = jobRdo.Fields.Get(Constants.Guids.Fields.InstanceMetricsJob.Metrics_MultipleChoice).ValueAsMultipleChoice;
				List<int> metricArtifactIdsToCollect = metricMultipleChoices.Select(x => x.ArtifactID).ToList();
				List<Guid> metricGuidsToCollect = ConvertChoiceArtifactIdsToGuids(servicesMgr, workspaceArtifactId, metricArtifactIdsToCollect);

				foreach (Guid metricGuid in metricGuidsToCollect)
				{
					ProcessSingleMetric(servicesMgr, workspaceArtifactId, jobArtifactId, metricGuid);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_ALL_JOB_METRICS_ERROR, ex);
			}
		}

		private void ProcessAllMetrics(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, InstanceMetricsJobObj jobRdo)
		{
			try
			{
				foreach (MetricsChoices metric in jobRdo.Metrics)
				{
					Guid metricGuid = metric.GetRelativityObjectAttributeGuidValue();

					ProcessSingleMetric(servicesMgr, workspaceArtifactId, jobArtifactId, metricGuid);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_ALL_JOB_METRICS_ERROR, ex);
			}
		}

		private List<Guid> ConvertChoiceArtifactIdsToGuids(IServicesMgr servicesMgr, int workspaceArtifactId, List<int> choiceArtifactIds)
		{
			try
			{
				List<Guid> choiceGuids = new List<Guid>();
				foreach (int choiceArtifactId in choiceArtifactIds)
				{
					Choice choice = _apiChooser.RetrieveMetricChoice(servicesMgr, workspaceArtifactId, choiceArtifactId);
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

		private void ProcessSingleMetric(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId, Guid metricGuid)
		{
			try
			{
				if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Workspaces))
				{
					ProcessWorkspacesMetric(servicesMgr, workspaceArtifactId, jobArtifactId);
				}
				else if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Users))
				{
					ProcessUsersMetric(servicesMgr, workspaceArtifactId, jobArtifactId);
				}
				else if (metricGuid.Equals(Constants.Guids.Choices.InstanceMetricsJob.Metrics_Groups))
				{
					ProcessGroupsMetric(servicesMgr, workspaceArtifactId, jobArtifactId);
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

		private void ProcessWorkspacesMetric(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing workspaces metric", 10);
				int noOfWorkspaces = _apiChooser.QueryNumberOfWorkspaces(servicesMgr);
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.WorkspacesCount_LongText, noOfWorkspaces);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_WORKSPACES_METRIC_ERROR, ex);
			}
		}

		private void ProcessUsersMetric(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing users metric", 10);
				int noOfUsers = _apiChooser.QueryNumberOfUsers(servicesMgr);
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.UsersCount_LongText, noOfUsers);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_USERS_METRIC_ERROR, ex);
			}
		}

		private void ProcessGroupsMetric(IServicesMgr servicesMgr, int workspaceArtifactId, int jobArtifactId)
		{
			try
			{
				RaiseMessage("Processing groups metric", 10);
				int noOfGroups = _apiChooser.QueryNumberOfGroups(servicesMgr);
				_apiChooser.UpdateJobField(servicesMgr, workspaceArtifactId, jobArtifactId, Constants.Guids.Fields.InstanceMetricsJob.GroupsCount_LongText, noOfGroups);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.PROCESS_GROUPS_METRIC_ERROR, ex);
			}
		}
	}
}
