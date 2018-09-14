using System;

namespace Helpers
{
	public class Constants
	{
		public class Guids
		{
			public static readonly Guid Application = new Guid("5BFE392E-2D1A-4023-BE44-4098C48E6976");
			public static readonly Guid AgentInstanceMetricsCalculator = new Guid("20530D16-D825-4FA5-9A7E-6760579EB07B");

			public class ObjectType
			{
				public static readonly Guid InstanceMetricsJob = new Guid("07FCE2E4-3318-4A00-9EF4-566FFCD7C198");
			}

			public class Fields
			{
				public class InstanceMetricsJob
				{
					public static readonly Guid ArtifactId_WholeNumber = new Guid("C62B7479-C80E-4653-93FB-D0F51D9E9058");
					public static readonly Guid Name_FixedLengthText = new Guid("7D1DFEDD-36A2-41A2-97D3-C1537DCD0598");
					public static readonly Guid Metrics_MultipleChoice = new Guid("70401A4A-94CC-45BB-A6CA-808F6754F114");
					public static readonly Guid Status_LongText = new Guid("065F1211-5A65-4DAC-AA26-EEED9007DCA9");
					public static readonly Guid Errors_LongText = new Guid("B9A22B34-0D87-4527-AD7F-B070AF4470AE");
					public static readonly Guid WorkspacesCount_LongText = new Guid("8435115F-894F-43C3-978E-8E9CF42AB2DB");
					public static readonly Guid UsersCount_LongText = new Guid("45D186F4-A9BB-4427-A6DA-DAA4AD639024");
					public static readonly Guid GroupsCount_LongText = new Guid("9B56A63F-5F42-4CBD-BE53-761E8AD32CA0");
				}
			}

			public class Choices
			{
				public class InstanceMetricsJob
				{
					public static readonly Guid Metrics_Workspaces = new Guid("9715EB01-97F0-496A-9640-2494AD7CAA35");
					public static readonly Guid Metrics_Users = new Guid("49BE6FCC-DB19-4BA3-A849-712DD2A72650");
					public static readonly Guid Metrics_Groups = new Guid("E9BCE5CE-EC87-4A46-AB61-E8157DC5BA57");
				}
			}
		}

		public class Names
		{
			public const string AGENT_INSTANCE_METRICS_CALCULATOR = "Instance Metrics Calculator Agent";
			public const string APPLICATION = "ADS Workshop Fest 2018";
		}

		public class JobStatus
		{
			public const string NEW = "New";
			public const string IN_PROGRESS = "In Progress";
			public const string COMPLETED = "Completed";
			public const string ERROR = "Error";
		}

		public class ErrorMessages
		{
			public const string QUERY_APPLICATION_WORKSPACES_ERROR = "An error occured when querying for workspaces where the application is installed";
			public const string QUERY_NUMBER_OF_WORKSPACES_ERROR = "An error occured when querying for number of workspaces in the instance";
			public const string QUERY_NUMBER_OF_USERS_ERROR = "An error occured when querying for number of users in the instance";
			public const string QUERY_NUMBER_OF_GROUPS_ERROR = "An error occured when querying for number of groups in the instance";
			public const string QUERY_APPLICATION_JOBS_ERROR = "An error occured when querying for jobs in a workspace";
			public const string RETRIEVE_APPLICATION_JOB_ERROR = "An error occured when retriving a job in a workspace";
			public const string RETRIEVE_METRIC_CHOICE_ERROR = "An error occured when retriving the metric choice";
			public const string UPDATE_APPLICATION_JOB_STATUS_ERROR = "An error occured when updating the Instance Metric Job field value";
			public const string EVENT_HANDLER_PRE_SAVE_ERROR = "An error occured when setting the job status in Pre-Save Eventhandler";
			public const string INVALID_METRIC_ERROR = "Invalid metric encountered";
			public const string PROCESS_SINGLE_JOB_METRIC_ERROR = "An error occured when processing metric for the job";
			public const string PROCESS_ALL_JOB_METRICS_ERROR = "An error occured when processing all metrics for the job";
			public const string PROCESS_SINGLE_WORKSPACE_ERROR = "An error occured when processing a workspace";
			public const string PROCESS_WORKSPACES_METRIC_ERROR = "An error occured when processing workspaces metric";
			public const string PROCESS_USERS_METRIC_ERROR = "An error occured when processing users metric";
			public const string PROCESS_GROUPS_METRIC_ERROR = "An error occured when processing groups metric";
			public const string CHOICE_ARTIFACT_ID_TO_GUID_CONVERSION_ERROR = "An error occured when Choice ArtifactIds to Guids";
			public const string RSAPI_HELPER_SETUP_ERROR = "An error occured when setting up RsapiHelper.";
		}
	}
}
