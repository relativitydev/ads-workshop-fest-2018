using Agents.Tests.Integration.Utility;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using UsernamePasswordCredentials = Relativity.Services.ServiceProxy.UsernamePasswordCredentials;

namespace Agents.Tests.Integration.Tests
{
	[TestFixture]
	[Description("Fixture description here")]
	public class AgentTests
	{
		private ServiceFactory _serviceFactory;
		private IServicesMgr _servicesManager;
		private IRSAPIClient _rsapiClient;
		private IDBContext _eddsDbContext;
		private AgentUtility _agentUtility;
		private TestHelper _testHelper;
		private readonly string _applicationRapFilePath = @"S:\SourceCode\GitHub\ads-workshop-fest-2018\SourceCode\ADS_Workshop_Starting\RAP\ADS_Workshop_Fest_2018.rap";
		private readonly string _workspaceName = "ADS" + "-" + Guid.NewGuid();
		private int _workspaceArtifactId;

		[OneTimeSetUp]
		public void Execute_OneTimeSetUpSetup()
		{
			Console.WriteLine("Start - OneTimeSetUp");

			//Setup Test variables
			SetupTestVariables();

			//Setup API Endpoints
			SetupApiEndpoints();

			//Create Workspace
			_workspaceArtifactId = CreateWorkspace(_workspaceName);

			//Install Application      
			InstallApplicationInWorkspace();

			//Setup Agents
			_agentUtility.SetUpAgents();

			Console.WriteLine("End - OneTimeSetUp");
		}

		[OneTimeTearDown]
		public void Execute_OneTimeTearDownTeardown()
		{
			//Delete Agents
			_agentUtility.CleanUpAgents();

			//Delete Workspace
			//DeleteWorkspace(_workspaceArtifactId);
		}

		[Test]
		[Description("Job_ShouldBePickedUp_WhenStatusIsSetToNew_And_RunSuccessfully")]
		public void Job_ShouldBePickedUp_WhenStatusIsSetToNew_And_RunSuccessfully()
		{
			int? testWorkspaceArtifactId = null;
			int? testUserArtifactId = null;
			int? testGroupArtifactId = null;
			int? jobArtifactId = null;

			try
			{
				// ARRANGE
				Console.WriteLine("Start - ARRANGE");

				//Get metrics before the job is run
				int numberOfWorkspacesBeforeJobRun = Helpers.RsapiHelper.QueryNumberOfWorkspaces(_servicesManager);
				Console.WriteLine($"{nameof(numberOfWorkspacesBeforeJobRun)}= {numberOfWorkspacesBeforeJobRun}");
				int numberOfUsersBeforeJobRun = Helpers.RsapiHelper.QueryNumberOfUsers(_servicesManager);
				Console.WriteLine($"{nameof(numberOfUsersBeforeJobRun)}= {numberOfUsersBeforeJobRun}");
				int numberOfGroupsBeforeJobRun = Helpers.RsapiHelper.QueryNumberOfGroups(_servicesManager);
				Console.WriteLine($"{nameof(numberOfGroupsBeforeJobRun)}= {numberOfGroupsBeforeJobRun}");

				//Create Test Workspace
				testWorkspaceArtifactId = CreateWorkspace($"Test_{Guid.NewGuid()}");
				Console.WriteLine($"{nameof(testWorkspaceArtifactId)}= {testWorkspaceArtifactId}");

				//Create Test User
				testUserArtifactId = CreateUser();
				Console.WriteLine($"{nameof(testUserArtifactId)}= {testUserArtifactId}");

				//Create Test Group
				testGroupArtifactId = CreateGroup($"Group_{Guid.NewGuid()}");
				Console.WriteLine($"{nameof(testGroupArtifactId)}= {testGroupArtifactId}");

				Console.WriteLine("End - ARRANGE");

				// ACT
				Console.WriteLine("Start - ACT");

				//Create job
				jobArtifactId = CreateJob(jobStatus: Helpers.Constants.JobStatus.NEW);
				Console.WriteLine($"{nameof(jobArtifactId)}= {jobArtifactId}");

				//Check if job has run
				string jobStatus = string.Empty;
				const int timeOutPeriod = 300; // 5 min
				int currentWaitTime = 0; // 0 secs
				int attempt = 1;
				while (currentWaitTime <= timeOutPeriod)
				{
					Console.WriteLine($"{nameof(attempt)}= {attempt}");

					//Check job status
					jobStatus = RetrieveJobStatus(jobArtifactId.Value);
					Console.WriteLine($"{nameof(jobStatus)}= {jobStatus}");

					if (jobStatus.Equals(Helpers.Constants.JobStatus.COMPLETED))
					{
						break;
					}

					Thread.Sleep(5000); // 5 secs
					currentWaitTime += 30;
					attempt++;
				}
				int numberOfWorkspacesAfterJobRun = Helpers.RsapiHelper.QueryNumberOfWorkspaces(_servicesManager);
				Console.WriteLine($"{nameof(numberOfWorkspacesAfterJobRun)}= {numberOfWorkspacesAfterJobRun}");
				int numberOfUsersAfterJobRun = Helpers.RsapiHelper.QueryNumberOfUsers(_servicesManager);
				Console.WriteLine($"{nameof(numberOfUsersAfterJobRun)}= {numberOfUsersAfterJobRun}");
				int numberOfGroupsAfterJobRun = Helpers.RsapiHelper.QueryNumberOfGroups(_servicesManager);
				Console.WriteLine($"{nameof(numberOfGroupsAfterJobRun)}= {numberOfGroupsAfterJobRun}");

				Console.WriteLine("End - ACT");

				// ASSERT
				Console.WriteLine("Start - ASSERT");
				Assert.That(jobStatus, Is.EqualTo(Helpers.Constants.JobStatus.COMPLETED));
				Assert.That(numberOfWorkspacesAfterJobRun, Is.EqualTo(numberOfWorkspacesBeforeJobRun + 1));
				Assert.That(numberOfUsersAfterJobRun, Is.EqualTo(numberOfUsersBeforeJobRun + 1));
				Assert.That(numberOfGroupsAfterJobRun, Is.EqualTo(numberOfGroupsBeforeJobRun + 1));
				Console.WriteLine("End - ASSERT");
			}
			finally
			{
				//Clean up
				DeleteWorkspace(testWorkspaceArtifactId);
				DeleteUser(testUserArtifactId);
				DeleteGroup(testGroupArtifactId);
				DeleteJob(jobArtifactId);
			}
		}

		[Test]
		[Description("Job_ShouldNotBePickedUp_WhenStatusIsNotSetToNew")]
		public void Job_ShouldNotBePickedUp_WhenStatusIsNotSetToNew()
		{
			int? jobArtifactId = null;

			try
			{
				// ARRANGE
				Console.WriteLine("Start - ARRANGE");
				Console.WriteLine("End - ARRANGE");

				// ACT
				Console.WriteLine("Start - ACT");

				//Create job
				jobArtifactId = CreateJob(jobStatus: string.Empty);
				Console.WriteLine($"{nameof(jobArtifactId)}= {jobArtifactId}");

				//Check if job has run
				string jobStatus = string.Empty;
				const int timeOutPeriod = 60; // 1 min
				int currentWaitTime = 0; // 0 secs
				int attempt = 1;
				while (currentWaitTime <= timeOutPeriod)
				{
					Console.WriteLine($"{nameof(attempt)}= {attempt}");

					//Check job status
					jobStatus = RetrieveJobStatus(jobArtifactId.Value);
					Console.WriteLine($"{nameof(jobStatus)}= {jobStatus}");

					if (jobStatus.Equals(Helpers.Constants.JobStatus.COMPLETED))
					{
						break;
					}

					Thread.Sleep(5000); // 5 secs
					currentWaitTime += 30;
					attempt++;
				}

				Console.WriteLine("End - ACT");

				// ASSERT
				Console.WriteLine("Start - ASSERT");
				Assert.That(jobStatus, Is.EqualTo(string.Empty));
				Console.WriteLine("End - ASSERT");
			}
			finally
			{
				//Clean up
				DeleteJob(jobArtifactId);
			}
		}

		private void SetupTestVariables()
		{
			Console.WriteLine("Start - Setup Test Variables.");

			//Setup variables
			_testHelper = new TestHelper();
			_servicesManager = _testHelper.GetServicesManager();
			_eddsDbContext = _testHelper.GetDBContext(-1);

			Console.WriteLine("End - Setup Test Variables.");
		}

		private void SetupApiEndpoints()
		{
			Console.WriteLine("Start - Setup API Endpoints.");

			UsernamePasswordCredentials usernamePasswordCredentials = new UsernamePasswordCredentials(TestConstants.InstanceDetails.RELATIVITY_ADMIN_USERNAME, TestConstants.InstanceDetails.RELATIVITY_ADMIN_PASSWORD);
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(TestConstants.InstanceDetails.RelativityServicesUri, TestConstants.InstanceDetails.RelativityRestUri, usernamePasswordCredentials);
			_serviceFactory = new ServiceFactory(serviceFactorySettings);
			_agentUtility = new AgentUtility(_eddsDbContext, _serviceFactory);
			_rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>();

			Console.WriteLine("End - Setup API Endpoints.");
		}

		private void InstallApplicationInWorkspace()
		{
			Console.WriteLine("Start - Install Application in Workspace.");

			Relativity.Test.Helpers.Application.ApplicationHelpers.ImportApplication(
				client: _rsapiClient,
				workspaceId: _workspaceArtifactId,
				forceFlag: true,
				filePath: _applicationRapFilePath,
				applicationName: Helpers.Constants.Names.APPLICATION);

			Console.WriteLine("End - Install Application in Workspace.");
		}

		private int CreateWorkspace(string workspaceName)
		{
			Console.WriteLine("Start - Create Workspace.");

			int workspaceArtifactId = -2;
			try
			{
				// retry logic for workspace creation
				int j = 1;

				while (j < TestConstants.WORKSPACE_CREATION_RETRY)
				{
					j++;
					try
					{
						Console.WriteLine("Creating workspace.");
						workspaceArtifactId = Relativity.Test.Helpers.WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(
								workspaceName: workspaceName,
								templateName: TestConstants.InstanceDetails.TEST_WORKSPACE_TEMPLATE_NAME,
								svcMgr: _servicesManager,
								userName: TestConstants.InstanceDetails.RELATIVITY_ADMIN_USERNAME,
								password: TestConstants.InstanceDetails.RELATIVITY_ADMIN_PASSWORD).Result;
						Console.WriteLine($"Workspace created [WorkspaceArtifactId= {workspaceArtifactId}]");
						j = 5;
					}
					catch (Exception e)
					{
						Console.WriteLine("Failed to create workspace, Retry now...");

						if (j != 5)
						{
							continue;
						}

						throw new Exception($"Failed to create workspace in the setup. Reset the Client to null\nError Message:\n{e.Message}.\nInner Exception Message:\n{e.InnerException.Message}.\nStrack trace:\n{e.StackTrace}.");
					}
				}

				return workspaceArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured while creating a new Workspace.", ex);
			}
			finally
			{
				Console.WriteLine("End - Create Workspace.");
			}
		}

		private bool DoesWorkspaceExists(int workspaceArtifactId)
		{
			Console.WriteLine("Start - Checking if workspace exists.");

			using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
			{
				rsapiClient.APIOptions.WorkspaceID = -1;

				try
				{
					rsapiClient.Repositories.Workspace.ReadSingle(workspaceArtifactId);
					Console.WriteLine("Workspace exists.");
					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					Console.WriteLine("End - Checking if workspace exists.");
				}
			}
		}

		private void DeleteWorkspace(int? workspaceArtifactId)
		{
			Console.WriteLine("Start - Deleting workspace.");

			if (workspaceArtifactId != null)
			{
				if (DoesWorkspaceExists(workspaceArtifactId.Value))
				{
					try
					{
						Console.WriteLine("Deleting workspace.");
						Relativity.Test.Helpers.WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(
							workspaceID: workspaceArtifactId.Value,
							svcMgr: _servicesManager,
							userName: TestConstants.InstanceDetails.RELATIVITY_ADMIN_USERNAME,
							password: TestConstants.InstanceDetails.RELATIVITY_ADMIN_PASSWORD);
						Console.WriteLine("Workspace Deleted.");
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when deleting a workspace", ex);
					}
				}
			}

			Console.WriteLine("End - Deleting workspace.");
		}

		private int CreateUser()
		{
			Console.WriteLine("Start - Creating User.");

			try
			{
				Console.WriteLine("Creating new User.");
				int userArtifactId = UserUtility.CreateUserUsingRepository(_rsapiClient);
				if (userArtifactId < 1)
				{
					throw new Exception("An error occured when creating a new User. Rsapi.");
				}

				return userArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating a new User", ex);
			}
			finally
			{
				Console.WriteLine("End - Creating User.");
			}
		}

		private bool DoesUserExists(int userArtifactId)
		{
			Console.WriteLine("Start - Checking if User exists.");

			using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
			{
				rsapiClient.APIOptions.WorkspaceID = -1;

				try
				{
					rsapiClient.Repositories.User.ReadSingle(userArtifactId);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					Console.WriteLine("End - Checking if User exists.");
				}
			}
		}

		private void DeleteUser(int? userArtifactId)
		{
			Console.WriteLine("Start - Deleting User.");

			if (userArtifactId != null)
			{
				if (DoesUserExists(userArtifactId.Value))
				{
					try
					{
						Console.WriteLine("Deleting User.");
						Relativity.Test.Helpers.UserHelpers.DeleteUser.Delete_User(_rsapiClient, userArtifactId.Value);
						Console.WriteLine("User Deleted.");
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when deleting a User", ex);
					}
				}
			}

			Console.WriteLine("End - Deleting User.");
		}

		private int CreateGroup(string groupName)
		{
			Console.WriteLine("Start - Creating Group.");

			try
			{
				Console.WriteLine("Creating new Group.");
				int groupArtifactId = Relativity.Test.Helpers.GroupHelpers.CreateGroup.Create_Group(_rsapiClient, groupName);
				Console.WriteLine("New Group Created.");
				return groupArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating a new Group", ex);
			}
			finally
			{
				Console.WriteLine("End - Creating Group.");
			}
		}

		private bool DoesGroupExists(int groupArtifactId)
		{
			Console.WriteLine("Start - Checking if Group exists.");

			using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
			{
				rsapiClient.APIOptions.WorkspaceID = -1;

				try
				{
					rsapiClient.Repositories.Group.ReadSingle(groupArtifactId);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					Console.WriteLine("End - Checking if Group exists.");
				}
			}
		}

		private void DeleteGroup(int? groupArtifactId)
		{
			Console.WriteLine("Start - Deleting Group.");

			if (groupArtifactId != null)
			{
				if (DoesGroupExists(groupArtifactId.Value))
				{
					try
					{
						Console.WriteLine("Deleting Group.");
						Relativity.Test.Helpers.GroupHelpers.DeleteGroup.Delete_Group(_rsapiClient, groupArtifactId.Value);
						Console.WriteLine("Group Deleted.");
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when deleting a Group", ex);
					}
				}
			}

			Console.WriteLine("End - Deleting Group.");
		}

		private int CreateJob(string jobStatus)
		{
			Console.WriteLine("Start - Creating Job.");

			try
			{
				using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = _workspaceArtifactId;
					string jobName = $"Job_{Guid.NewGuid()}";
					MultiChoiceFieldValueList multiChoices = new MultiChoiceFieldValueList(
						new kCura.Relativity.Client.DTOs.Choice(Helpers.Constants.Guids.Choices.InstanceMetricsJob.Metrics_Workspaces),
						new kCura.Relativity.Client.DTOs.Choice(Helpers.Constants.Guids.Choices.InstanceMetricsJob.Metrics_Users),
						new kCura.Relativity.Client.DTOs.Choice(Helpers.Constants.Guids.Choices.InstanceMetricsJob.Metrics_Groups))
					{
						UpdateBehavior = MultiChoiceUpdateBehavior.Replace
					};

					RDO jobRdo = new RDO
					{
						ArtifactTypeGuids = new List<Guid> { Helpers.Constants.Guids.ObjectType.InstanceMetricsJob },
						Fields = new List<FieldValue>
						{
							new FieldValue(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Name_FixedLengthText, jobName),
							new FieldValue(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Status_LongText, jobStatus),
							new FieldValue(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Metrics_MultipleChoice, multiChoices)
						}
					};

					try
					{
						int jobArtifactId = rsapiClient.Repositories.RDO.CreateSingle(jobRdo);
						return jobArtifactId;
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when creating a job. CreateSingle.", ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating a job.", ex);
			}
			finally
			{
				Console.WriteLine("End - Creating Job.");
			}
		}

		private bool DoesJobExists(int jobArtifactId)
		{
			Console.WriteLine("Start - Checking if Job exists.");

			using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
			{
				rsapiClient.APIOptions.WorkspaceID = _workspaceArtifactId;

				try
				{
					rsapiClient.Repositories.RDO.ReadSingle(jobArtifactId);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					Console.WriteLine("End - Checking if Job exists.");
				}
			}
		}

		private void DeleteJob(int? jobArtifactId)
		{
			Console.WriteLine("Start - Deleting Job.");

			if (jobArtifactId != null)
			{
				if (DoesJobExists(jobArtifactId.Value))
				{
					try
					{
						using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
						{
							rsapiClient.APIOptions.WorkspaceID = _workspaceArtifactId;
							try
							{
								rsapiClient.Repositories.RDO.DeleteSingle(jobArtifactId.Value);
							}
							catch (Exception ex)
							{
								throw new Exception("An error occured when deleting a job. DeleteSingle.", ex);
							}
						}
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when deleting a job.", ex);
					}
				}
			}

			Console.WriteLine("End - Deleting Job.");
		}

		private string RetrieveJobStatus(int jobArtifactId)
		{
			Console.WriteLine("Start - Retrieving Job Status.");

			try
			{
				using (IRSAPIClient rsapiClient = _serviceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = _workspaceArtifactId;
					RDO jobRdo;
					try
					{
						jobRdo = rsapiClient.Repositories.RDO.ReadSingle(jobArtifactId);
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when retrieving job status. ReadSingle.", ex);
					}

					string jobStatus = jobRdo.Fields.Get(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Status_LongText).ValueAsLongText;
					return jobStatus;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when retrieving job status.", ex);
			}
			finally
			{
				Console.WriteLine("End - Retrieving Job Status.");
			}
		}
	}
}