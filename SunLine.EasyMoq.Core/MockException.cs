using System;

namespace SunLine.EasyMoq.Core
{
	public class MockException : Exception
	{
		internal enum ExceptionReason
		{
			NoSetup,
			ReturnValueRequired,
			VerificationFailed,
			MoreThanOneCall,
			MoreThanNCalls,
			SetupNever
		}

		private ExceptionReason reason;

		internal MockException(ExceptionReason reason, string exceptionMessage)
			: base(exceptionMessage)
		{
			this.reason = reason;
		}

		internal ExceptionReason Reason
		{
			get { return reason; }
		}
	}
}