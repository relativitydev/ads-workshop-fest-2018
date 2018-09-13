using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Helpers.Tests.Unit
{
	[TestFixture]
	[Description("Rsapi Helper Tests")]
	public class RsapiHelperTests
	{
		public RsapiHelper Sut { get; set; }
		public Mock<IGenericRepository<RDO>> MockRdoRepository { get; set; }

		[OneTimeSetUp]
		public void Execute_OneTimeSetUpSetup()
		{
			Console.WriteLine("Start - OneTimeSetUp");

			MockRdoRepository = new Mock<IGenericRepository<RDO>>();
			APIOptions rsapiApiOptions = new APIOptions
			{
				WorkspaceID = -1
			};
			Sut = new RsapiHelper
			{
				RsapiApiOptions = rsapiApiOptions,
				RdoRepository = MockRdoRepository.Object
			};

			Console.WriteLine("End - OneTimeSetUp");
		}

		[OneTimeTearDown]
		public void Execute_OneTimeTearDownTeardown()
		{
			//Delete Agents
			//_agentUtility.CleanUpAgents();

			//Delete Workspace
			//DeleteWorkspace(_workspaceArtifactId);
		}

		#region TestConstants

		private const int TestWorkspaceArtifactId = 123;
		private const string TestStatus = "New";

		#endregion

		[Test]
		public void RetrieveJobsInWorkspaceWithStatus_GoldenFlow()
		{
			//Arrange
			int rdoCount = 5;
			Mock_RdoRepository_Query_Works(rdoCount);

			//Act
			List<int> jobsList = Sut.RetrieveJobsInWorkspaceWithStatus(TestWorkspaceArtifactId, TestStatus);

			//Assert
			Verify_RdoRepository_Query_Works_Was_Called(1);
			Assert.That(jobsList.Count, Is.EqualTo(rdoCount));
		}

		[Test]
		public void RetrieveJobsInWorkspaceWithStatus_Rsapi_Fails()
		{
			//Arrange
			Mock_RdoRepository_Query_Throws_Exception();

			//Act
			Exception exception = Assert.Throws<Exception>(() => Sut.RetrieveJobsInWorkspaceWithStatus(TestWorkspaceArtifactId, TestStatus));

			//Assert
			StringAssert.Contains(Constants.ErrorMessages.QUERY_APPLICATION_JOBS_ERROR, exception.ToString());
			Verify_RdoRepository_Query_Works_Was_Called(1);
		}

		private void Mock_RdoRepository_Query_Works(int rdoCount)
		{
			List<Result<RDO>> results = new List<Result<RDO>>();
			for (int i = 1; i <= rdoCount; i++)
			{
				Result<RDO> newResult = new Result<RDO>
				{
					Artifact = new RDO(i),
					Message = string.Empty,
					Success = true
				};
				results.Add(newResult);
			}
			QueryResultSet<RDO> rdoQueryResultSet = new QueryResultSet<RDO>
			{
				Success = true,
				Results = results
			};

			MockRdoRepository
				.Setup(x => x.Query(It.IsAny<Query<RDO>>(), It.IsAny<int>()))
				.Returns(rdoQueryResultSet);
		}

		private void Mock_RdoRepository_Query_Throws_Exception()
		{
			MockRdoRepository
				.Setup(x => x.Query(It.IsAny<Query<RDO>>(), It.IsAny<int>()))
				.Throws<Exception>();
		}

		private void Verify_RdoRepository_Query_Works_Was_Called(int timesCalled)
		{
			MockRdoRepository
				.Verify(x => x.Query(It.IsAny<Query<RDO>>(), It.IsAny<int>())
				, Times.Exactly(timesCalled));
		}
	}
}
