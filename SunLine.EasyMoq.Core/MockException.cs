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

        internal ExceptionReason Reason { get; }

        internal MockException(ExceptionReason reason, string exceptionMessage)
            : base(exceptionMessage)
        {
            Reason = reason;
        }
    }
}