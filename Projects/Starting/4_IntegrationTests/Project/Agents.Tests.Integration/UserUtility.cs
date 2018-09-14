using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agents.Tests.Integration
{
	public class UserUtility
	{
		public static int CreateUserUsingRepository(IRSAPIClient rsapiClient)
		{
			rsapiClient.APIOptions.WorkspaceID = -1;

			int newUserArtifactId = -1;
			const int defaultSelectedFileType = 1;
			const int userType = 3;
			const int documentSkip = 1000003;
			const int skipDefaultPreference = 1000004;
			const int password = 1000005;
			const int sendNewPasswordTo = 1000006;

			int returnPasswordCodeId = FindChoiceArtifactId(rsapiClient, sendNewPasswordTo, "Return");
			int passwordCodeId = FindChoiceArtifactId(rsapiClient, password, "Auto-generate password");
			int documentSkipCodeId = FindChoiceArtifactId(rsapiClient, documentSkip, "Enabled");

			int documentSkipPreferenceCodeId = FindChoiceArtifactId(rsapiClient, skipDefaultPreference, "Normal");
			int defaultFileTypeCodeId = FindChoiceArtifactId(rsapiClient, defaultSelectedFileType, "Native");
			int userTypeCodeId = FindChoiceArtifactId(rsapiClient, userType, "Internal");

			int everyoneGroupArtifactId = FindGroupArtifactId(rsapiClient, "Everyone");
			int clientArtifactId = FindClientArtifactId(rsapiClient, "Relativity Template");

			long ticks = DateTime.Now.Ticks;
			kCura.Relativity.Client.DTOs.User userDto = new kCura.Relativity.Client.DTOs.User
			{
				AdvancedSearchPublicByDefault = true,
				AuthenticationData = "",
				BetaUser = false,
				ChangePassword = true,
				ChangePasswordNextLogin = false,
				ChangeSettings = true,
				Client = new Client(clientArtifactId),
				DataFocus = 1,
				DefaultSelectedFileType = new kCura.Relativity.Client.DTOs.Choice(defaultFileTypeCodeId),
				DocumentSkip = new kCura.Relativity.Client.DTOs.Choice(documentSkipCodeId),
				EmailAddress = $"email.{ticks}@test.com",
				EnforceViewerCompatibility = true,
				FirstName = $"firstName_{ticks}",
				Groups = new List<Group> { new Group(everyoneGroupArtifactId) },
				ItemListPageLength = 25,
				KeyboardShortcuts = true,
				LastName = "Test User",
				MaximumPasswordAge = 0,
				NativeViewerCacheAhead = true,
				PasswordAction = new kCura.Relativity.Client.DTOs.Choice(passwordCodeId),
				RelativityAccess = true,
				SendPasswordTo = new kCura.Relativity.Client.DTOs.Choice(returnPasswordCodeId),
				SkipDefaultPreference = new kCura.Relativity.Client.DTOs.Choice(documentSkipPreferenceCodeId),
				TrustedIPs = "",
				Type = new kCura.Relativity.Client.DTOs.Choice(userTypeCodeId)
			};

			WriteResultSet<kCura.Relativity.Client.DTOs.User> userWriteResultSet = new WriteResultSet<kCura.Relativity.Client.DTOs.User>();

			try
			{
				userWriteResultSet = rsapiClient.Repositories.User.Create(userDto);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}

			// Check for success.
			if (!userWriteResultSet.Success)
			{
				Console.WriteLine($"An error occurred creating user: {userWriteResultSet.Message}");

				foreach (Result<kCura.Relativity.Client.DTOs.User> createResult in userWriteResultSet.Results)
				{
					if (!createResult.Success)
					{
						Console.WriteLine($"An error occurred in create request: {createResult.Message}");
					}
				}
			}

			if (userWriteResultSet.Results != null)
			{
				newUserArtifactId = userWriteResultSet.Results.FirstOrDefault().Artifact.ArtifactID;
			}
			return newUserArtifactId;
		}

		private static int FindChoiceArtifactId(IRSAPIClient rsapiClient, int choiceType, string value)
		{
			int artifactId = 0;

			WholeNumberCondition choiceTypeCondition = new WholeNumberCondition(ChoiceFieldNames.ChoiceTypeID, NumericConditionEnum.EqualTo, (int)choiceType);
			TextCondition choiceNameCondition = new TextCondition(ChoiceFieldNames.Name, TextConditionEnum.EqualTo, value);
			CompositeCondition choiceCompositeCondition = new CompositeCondition(choiceTypeCondition, CompositeConditionEnum.And, choiceNameCondition);
			Query<kCura.Relativity.Client.DTOs.Choice> choiceQuery = new Query<kCura.Relativity.Client.DTOs.Choice>(
				new List<FieldValue>
				{
					new FieldValue(ArtifactQueryFieldNames.ArtifactID)
				},
				choiceCompositeCondition,
				new List<Sort>());

			try
			{
				QueryResultSet<kCura.Relativity.Client.DTOs.Choice> choiceQueryResult = rsapiClient.Repositories.Choice.Query(choiceQuery);

				if (choiceQueryResult.Success && choiceQueryResult.Results.Count == 1)
				{
					artifactId = choiceQueryResult.Results.FirstOrDefault().Artifact.ArtifactID;
				}
				else
				{
					Console.WriteLine("The choice could not be found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
			return artifactId;
		}

		private static int FindGroupArtifactId(IRSAPIClient rsapiClient, string group)
		{
			int artifactId = 0;

			TextCondition groupCondition = new TextCondition(GroupFieldNames.Name, TextConditionEnum.EqualTo, group);

			Query<Group> queryGroup = new Query<Group>
			{
				Condition = groupCondition
			};

			queryGroup.Fields.Add(new FieldValue(ArtifactQueryFieldNames.ArtifactID));

			try
			{
				QueryResultSet<Group> resultSetGroup = rsapiClient.Repositories.Group.Query(queryGroup, 0);

				if (resultSetGroup.Success && resultSetGroup.Results.Count == 1)
				{
					artifactId = resultSetGroup.Results.FirstOrDefault().Artifact.ArtifactID;
				}
				else
				{
					Console.WriteLine("The Query operation failed.{0}{1}", Environment.NewLine, resultSetGroup.Message);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
			return artifactId;
		}

		private static int FindClientArtifactId(IRSAPIClient rsapiClient, string group)
		{
			int artifactId = 0;

			TextCondition clientCondition = new TextCondition(ClientFieldNames.Name, TextConditionEnum.EqualTo, group);

			Query<Client> queryClient = new Query<Client>
			{
				Condition = clientCondition,
				Fields = FieldValue.AllFields
			};

			try
			{
				QueryResultSet<Client> resultSetClient = rsapiClient.Repositories.Client.Query(queryClient, 0);

				if (resultSetClient.Success && resultSetClient.Results.Count == 1)
				{
					artifactId = resultSetClient.Results.FirstOrDefault().Artifact.ArtifactID;
				}
				else
				{
					Console.WriteLine("The Query operation failed.{0}{1}", Environment.NewLine, resultSetClient.Message);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred: {ex.Message}");
			}
			return artifactId;
		}
	}
}
