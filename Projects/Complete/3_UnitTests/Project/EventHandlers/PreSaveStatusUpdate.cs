using kCura.EventHandler;
using System;

namespace EventHandlers
{
	[kCura.EventHandler.CustomAttributes.Description("Sets the job status to New")]
	[System.Runtime.InteropServices.Guid("2F17402F-D0D8-46E8-9324-C0C863043BA4")]
	public class PreSaveStatusUpdate : PreSaveEventHandler
	{
		public override Response Execute()
		{
			try
			{
				Response response = new Response
				{
					Success = true,
					Message = string.Empty
				};

				int statusFieldArtifactId = GetArtifactIdByGuid(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Status_LongText);

				if (ActiveArtifact.IsNew)
				{
					//Update the Status field
					ActiveArtifact.Fields[statusFieldArtifactId].Value.Value = Helpers.Constants.JobStatus.NEW;
				}

				return response;
			}
			catch (Exception ex)
			{
				throw new Exception(Helpers.Constants.ErrorMessages.EVENT_HANDLER_PRE_SAVE_ERROR, ex);
			}
		}

		public override FieldCollection RequiredFields
		{
			get
			{
				FieldCollection fieldCollection = new FieldCollection
				{
					new Field(Helpers.Constants.Guids.Fields.InstanceMetricsJob.Status_LongText)
				};
				return fieldCollection;
			}
		}
	}
}
