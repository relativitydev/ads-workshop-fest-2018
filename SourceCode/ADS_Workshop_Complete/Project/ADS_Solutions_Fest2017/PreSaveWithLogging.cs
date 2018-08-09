using kCura.EventHandler;

namespace ADS_Solutions_Fest2017
{
    [kCura.EventHandler.CustomAttributes.Description("Event Handler With Logging")]
    [System.Runtime.InteropServices.Guid("cee4ba4a-b69e-41f7-8887-efb532661291")]
    public class PreSaveWithLogging : kCura.EventHandler.PreSaveEventHandler
    {
        public override kCura.EventHandler.Response Execute()
        {
            Response retVal = new kCura.EventHandler.Response();
            try
            {
                Helper.GetLoggerFactory().GetLogger().LogDebug("About to try something!");
                throw new System.Exception("Boom");
            }
            catch (System.Exception ex)
            {
                Helper.GetLoggerFactory().GetLogger().LogError("That definitly did not work", ex);
                retVal.Success = false;
                retVal.Message = ex.ToString();
            }

            return retVal;
        }

        /// <summary>
        /// The RequiredFields property tells Relativity that your event handler needs to have access to specific fields that you return in this collection property
        /// regardless if they are on the current layout or not. These fields will be returned in the ActiveArtifact.Fields collection just like other fields that are on
        /// the current layout when the event handler is executed.
        /// </summary>
        public override kCura.EventHandler.FieldCollection RequiredFields
        {
            get
            {
                kCura.EventHandler.FieldCollection retVal = new kCura.EventHandler.FieldCollection();
                return retVal;
            }
        }
    }
}
