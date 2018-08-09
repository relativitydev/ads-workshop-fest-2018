using System;

namespace OverrideCustomPage.Helpers
{
    public class Constants
    {
        public const string ApplicationName = "ADS Solutions Fest 2017";

        public class Guids
        {
            public static readonly Guid Application = new Guid("21D274EA-F22D-428D-A1CE-959F3CBDD6DC");
            public static readonly Guid CustomPage = new Guid("E374B4D2-04D1-4C31-A3EC-58BE519DDE6C");

            public class ObjectTypes
            {
                public static readonly Guid OverrideRdo = new Guid("A0B5E207-6F95-4CE8-A18A-D323DE09BE79");
            }

            public class Fields
            {
                public class OverrideRdo
                {
                    public static readonly Guid ArtifactId = new Guid("24514650-CBDA-47F1-94A0-47763ECB2172");
                    public static readonly Guid Name = new Guid("614215D8-19C4-41AA-97F0-68BC09F8202C");
                    public static readonly Guid Phone = new Guid("50BBF393-E4E8-4BD9-AE8B-D7FCC450AAAC");
                    public static readonly Guid Email = new Guid("7C38D7AD-4B39-4DED-A7AD-AB7D38A79B05");
                }
            }
        }

        public class FieldNames
        {
            public class OverrideRdo
            {
                public static readonly string Name = "Name";
                public static readonly string Phone = "Phone";
                public static readonly string Email = "Email";
            }
        }

        public class ResultMessages
        {
            public const string NewRdoSuccess = "Your new record was saved successfully.";
            public const string NewRdoFail = "Your new record could not be saved. Try again.";
            public const string EditRdoSuccess = "Your record was updated successfully.";
            public const string EditRdoFail = "Your record could not be udpated. Try again.";
        }

        public class ErrorMessages
        {
            public const string NewRdoCreateError = "An error occured when saving new RDO record";
            public const string RdoReadError = "An error occured when querying for retrieving current RDO values";
            public const string EditRdoSaveError = "An error occured when updating RDO with new values";
        }

        public class ViewHeaderText
        {
            public const string NewRdo = "New RDO";
            public const string EditRdo = "Edit RDO";
        }
    }
}