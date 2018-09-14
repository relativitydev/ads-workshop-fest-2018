using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Helpers
{
	public class SqlHelper
	{
		public static DataTable RetrieveApplicationWorkspaces(IDBContext eddsDbContext, Guid applicationGuid)
		{
			try
			{
				const string sql = @"DECLARE @appArtifactID INT
						SET @appArtifactID = (SELECT ArtifactID FROM ArtifactGuid WHERE ArtifactGuid = @appGuid)

						SELECT  C.ArtifactID, C.Name
						FROM CaseApplication (NOLOCK) CA
						 INNER JOIN eddsdbo.[ExtendedCase] C ON CA.CaseID = C.ArtifactID
						 INNER JOIN eddsdbo.ResourceServer RS ON C.ServerID = RS.ArtifactID
						 INNER JOIN eddsdbo.Artifact A (NOLOCK) ON C.ArtifactID = A.ArtifactID
						 INNER JOIN eddsdbo.[ApplicationInstall] as AI on CA.CurrentApplicationInstallID = AI.ApplicationInstallID
						WHERE CA.ApplicationID = @appArtifactId
							AND AI.[Status] = 6 --Installed
						ORDER BY A.CreatedOn
						";

				List<SqlParameter> sqlParams = new List<SqlParameter>
				{
					new SqlParameter("@appGuid", SqlDbType.UniqueIdentifier) {Value = applicationGuid}
				};

				return eddsDbContext.ExecuteSqlStatementAsDataTable(sql, sqlParams);
			}
			catch (Exception ex)
			{
				throw new Exception(Constants.ErrorMessages.QUERY_APPLICATION_WORKSPACES_ERROR, ex);
			}
		}
	}
}
