using Relativity.Services.Agent;
using System;

namespace Agents.Tests.Integration
{
	public class TestConstants
	{
		//todo: move InstanceDetails to app.config
		public class InstanceDetails
		{
			public const string INSTANCE_NAME = "192.168.137.95";
			public const string PROTOCOL = "http";
			public const string RELATIVITY_ADMIN_USERNAME = "relativity.admin@relativity.com";
			public const string RELATIVITY_ADMIN_PASSWORD = "Test1234!";
			public const string TEST_WORKSPACE_TEMPLATE_NAME = "Relativity Starter Template";
			public const string SQL_SERVER_NAME = "192.168.137.95";
			public const string SQL_DATABASE_NAME = "EDDS";
			public const string SQL_USERNAME = "eddsdbo";
			public const string SQL_PASSWORD = "Test1234!";
			public static readonly Uri RelativityServicesUri = new Uri($"{PROTOCOL}://{INSTANCE_NAME}/Relativity.Services");
			public static readonly Uri RelativityRestUri = new Uri($"{PROTOCOL}://{INSTANCE_NAME}/Relativity.Rest/Api");
		}

		public const string AGENT_OFF_HOURS_SECTION_NAME = "kCura.EDDS.Agents";
		public const string AGENT_OFF_HOUR_START_TIME_NAME = "AgentOffHourStartTime";
		public const string AGENT_OFF_HOUR_END_TIME_NAME = "AgentOffHourEndTime";
		public const bool ENABLE_AGENT = true;
		public const int AGENT_INTERVAL = 10;
		public const Agent.LoggingLevelEnum AGENT_LOGGING_LEVEL = Agent.LoggingLevelEnum.All;
		public const int WORKSPACE_CREATION_RETRY = 3;
	}
}
