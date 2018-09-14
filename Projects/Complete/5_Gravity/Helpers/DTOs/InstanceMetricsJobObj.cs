using Gravity.Base;
using System;
using System.Collections.Generic;

namespace Helpers.DTOs
{
	[Serializable]
	[RelativityObject("07FCE2E4-3318-4A00-9EF4-566FFCD7C198")]
	public class InstanceMetricsJobObj : BaseDto
	{
		[RelativityObjectField("7D1DFEDD-36A2-41A2-97D3-C1537DCD0598", RdoFieldType.FixedLengthText)]
		public override string Name { get; set; }

		[RelativityObjectField("065F1211-5A65-4DAC-AA26-EEED9007DCA9", RdoFieldType.LongText)]
		public string Status { get; set; }

		[RelativityObjectField("70401A4A-94CC-45BB-A6CA-808F6754F114", RdoFieldType.MultipleChoice)]
		public IList<MetricsChoices> Metrics { get; set; }

		[RelativityObjectField("8435115F-894F-43C3-978E-8E9CF42AB2DB", RdoFieldType.LongText)]
		public string WorkspacesCount { get; set; }

		[RelativityObjectField("45D186F4-A9BB-4427-A6DA-DAA4AD639024", RdoFieldType.LongText)]
		public string UsersCount { get; set; }

		[RelativityObjectField("9B56A63F-5F42-4CBD-BE53-761E8AD32CA0", RdoFieldType.LongText)]
		public string GroupsCount { get; set; }

		[RelativityObjectField("B9A22B34-0D87-4527-AD7F-B070AF4470AE", RdoFieldType.LongText)]
		public string Errors { get; set; }
	}
}
