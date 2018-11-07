﻿using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Objects;
using System;
using System.Web.Mvc;

namespace AdsWorkshopFest2018.Controllers
{
	public class HomeController : Controller
	{
		// GET: Home
		public ActionResult Index()
		{
			IAPILog logger = Relativity.CustomPages.ConnectionHelper.Helper().GetLoggerFactory().GetLogger();

			try
			{
				//Gets the user ID.
				int userArtifactId = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.ArtifactID;
				//Gets the user ID to use for auditing purposes.
				int userAuditArtifactId = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.AuditArtifactID;
				//Gets the user workspace artifact ID to use for auditing purposes.
				int userAuditWorkspaceUserArtifactId = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.AuditWorkspaceUserArtifactID;
				//Gets the email address of the current user.
				string userEmailAddress = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.EmailAddress;
				//Gets the first name of the current user.
				string firstName = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.FirstName;
				//Gets the last name of the current user.
				string lastName = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.LastName;
				//Gets the full name of the current user.
				string fullName = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.FullName;
				//Gets the current user workspace artifact ID.
				int currentUserWorkspaceArtifactId = Relativity.CustomPages.ConnectionHelper.Helper().GetAuthenticationManager().UserInfo.WorkspaceUserArtifactID;

				//Get GUID for an artifact
				int testArtifactId = 1234567;
				Guid guidForTestArtifactId = Relativity.CustomPages.ConnectionHelper.Helper().GetGuid(currentUserWorkspaceArtifactId, testArtifactId);

				//Get a dbContext for the EDDS database
				IDBContext eddsDbContext = Relativity.CustomPages.ConnectionHelper.Helper().GetDBContext(-1);

				//Get a dbContext for the workspace database
				IDBContext workspaceDbContext = Relativity.CustomPages.ConnectionHelper.Helper().GetDBContext(currentUserWorkspaceArtifactId);

				//The Object Manager is the newest and preferred way to interact with Relativity instead of the Relativity Services API(RSAPI). 
				//The RSAPI will be scheduled for depreciation after the Object Manager reaches feature party with it.
				using (IObjectManager objectManager = Relativity.CustomPages.ConnectionHelper.Helper().GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{

				}

				//Setting up an RSAPI Client
				using (IRSAPIClient rsapiClient = Relativity.CustomPages.ConnectionHelper.Helper().GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.CurrentUser))
				{
					//Set the proxy to use the Admin case
					rsapiClient.APIOptions.WorkspaceID = -1;

					kCura.Relativity.Client.DTOs.Query<kCura.Relativity.Client.DTOs.Workspace> workspaceQuery = new kCura.Relativity.Client.DTOs.Query<kCura.Relativity.Client.DTOs.Workspace>();

					workspaceQuery.Fields.Add(new kCura.Relativity.Client.DTOs.FieldValue(kCura.Relativity.Client.DTOs.WorkspaceFieldNames.TextIdentifier));

					kCura.Relativity.Client.DTOs.QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> workspaceQueryResultSet = new kCura.Relativity.Client.DTOs.QueryResultSet<kCura.Relativity.Client.DTOs.Workspace>();

					try
					{
						workspaceQueryResultSet = rsapiClient.Repositories.Workspace.Query(workspaceQuery, 0);
					}
					catch (Exception ex)
					{
						throw new Exception("An error occured when querying for workspaces. Query.", ex);
					}

					int workspaceCount = workspaceQueryResultSet.Results.Count;
					ViewBag.WorkspaceCount = workspaceCount;
				}

				logger.LogVerbose("Log information throughout execution.");
			}
			catch (Exception ex)
			{
				//Your custom page caught an exception
				logger.LogError(ex, "There was an exception.");
			}

			return View();
		}
	}
}