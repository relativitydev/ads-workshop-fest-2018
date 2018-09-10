using System;
using System.Text;

namespace Helpers
{
	public class ExceptionMessageFormatter
	{
		public static string GetInnerMostExceptionMessage(Exception exception)
		{
			string retVal;

			if (exception == null)
			{
				retVal = string.Empty;
			}
			else
			{
				Exception currentException = exception;
				while (currentException.InnerException != null)
				{
					currentException = currentException.InnerException;
				}

				retVal = currentException.Message;
			}
			return retVal;
		}

		public static string GetExceptionMessageIncludingAllInnerExceptions(Exception exception)
		{
			string retVal;

			if (exception == null)
			{
				retVal = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				Exception currentException = exception;
				stringBuilder.Append(currentException.Message);

				while (currentException.InnerException != null)
				{
					currentException = currentException.InnerException;
					stringBuilder.Append(" --> " + currentException.Message);
				}

				retVal = stringBuilder.ToString();
			}
			return retVal;
		}
	}
}
