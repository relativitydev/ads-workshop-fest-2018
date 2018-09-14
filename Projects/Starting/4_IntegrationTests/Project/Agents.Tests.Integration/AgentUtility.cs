using System;
using System.Collections.Generic;
using System.Linq;
using AgentUtilities;
using Relativity.API;
using Relativity.Services.Agent;
using Relativity.Services.ServiceProxy;

namespace Agents.Tests.Integration
{
	public class AgentUtility
	{
		private readonly IDBContext _eddsDbContext;
		private readonly ServiceFactory _serviceFactory;

		public AgentUtility(IDBContext eddsDbContext, ServiceFactory serviceFactory)
		{
			_eddsDbContext = eddsDbContext;
			_serviceFactory = serviceFactory;
		}

		public void SetUpAgents()
		{
			SetAgentOffHours(TestConstants.AGENT_OFF_HOUR_START_TIME_NAME, "00:00:00");
			SetAgentOffHours(TestConstants.AGENT_OFF_HOUR_END_TIME_NAME, "11:59:59");
			CreateAgents();
		}

		public void CleanUpAgents()
		{
			DeleteAgents();
		}

		private void SetAgentOffHours(string agentOffHoursType, string timeValue)
		{
			string sql = $@"
				UPDATE 
					[EDDSDBO].[Configuration]
				SET 
					[Value] = '{timeValue}' 
				WHERE
					Section = '{TestConstants.AGENT_OFF_HOURS_SECTION_NAME}' AND
					[Name] = '{agentOffHoursType}'
				";

			try
			{
				_eddsDbContext.ExecuteNonQuerySQLStatement(sql);
			}
			catch (Exception ex)
			{
				throw new Exception($"An error occured when setting the Agent OffHours time of type: {agentOffHoursType}.", ex);
			}
		}

		private void CreateAgents()
		{
			try
			{
				using (IAgentManager agentManager = _serviceFactory.CreateProxy<IAgentManager>())
				{
					AgentUtilities.IAgentHelper agentHelper = new AgentHelper(
						agentManager: agentManager,
						sqlDatabaseServerName: TestConstants.InstanceDetails.SQL_SERVER_NAME,
						sqlDatabaseName: TestConstants.InstanceDetails.SQL_DATABASE_NAME,
						sqlUsername: TestConstants.InstanceDetails.SQL_USERNAME,
						sqlPassword: TestConstants.InstanceDetails.SQL_PASSWORD);

					// Check if Agent exists in the instance
					int agentArtifactTypeId = agentHelper.GetAgentArtifactTypeIdByNameAsync(Helpers.Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR).Result;
					int agentServerArtifactId = agentHelper.GetAgentServerByResourceServerTypeAsync("Agent").Result;
					List<Agent> agentList = agentHelper.GetAgentByNameAsync(Helpers.Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR).Result;

					if (agentList.Count == 0)
					{
						//Create Agent
						Console.WriteLine("Creating Agent.");
						int newAgentArtifactId = agentHelper.CreateAgentAsync(
							agentName: Helpers.Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR,
							agentTypeId: agentArtifactTypeId,
							agentServer: agentServerArtifactId,
							enableAgent: TestConstants.ENABLE_AGENT,
							agentInterval: TestConstants.AGENT_INTERVAL,
							agentLoggingLevel: TestConstants.AGENT_LOGGING_LEVEL).Result;
						Console.WriteLine($"Agent Created. {nameof(newAgentArtifactId)} = {newAgentArtifactId}");
					}
					else
					{
						Console.WriteLine("Agent exists in the Instance. Skipped creation.");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error encountered while creating Agents.", ex);
			}
		}

		private void DeleteAgents()
		{
			try
			{
				using (IAgentManager agentManager = _serviceFactory.CreateProxy<IAgentManager>())
				{
					AgentUtilities.IAgentHelper agentHelper = new AgentHelper(
						agentManager: agentManager,
						sqlDatabaseServerName: TestConstants.InstanceDetails.SQL_SERVER_NAME,
						sqlDatabaseName: TestConstants.InstanceDetails.SQL_DATABASE_NAME,
						sqlUsername: TestConstants.InstanceDetails.SQL_USERNAME,
						sqlPassword: TestConstants.InstanceDetails.SQL_PASSWORD);

					// Check if Agent exists in the instance
					List<Agent> agentsList = agentHelper.GetAgentByNameAsync(Helpers.Constants.Names.AGENT_INSTANCE_METRICS_CALCULATOR).Result;

					if (agentsList.Count > 0)
					{
						//Delete Agent(s)
						Console.WriteLine("Deleting Agent.");
						List<int> agentArtifactIds = agentsList.Select(x => x.ArtifactID).ToList();
						agentHelper.DeleteMultipleAgentsAsync(agentArtifactIds).Wait();
						Console.WriteLine($"Agent(s) Deleted. Count = {agentsList.Count}");
					}
					else
					{
						Console.WriteLine("Agent(s) doesn't exists in the Instance. Skipped deletion.");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error encountered while deleting Agents.", ex);
			}
		}
	}
}
