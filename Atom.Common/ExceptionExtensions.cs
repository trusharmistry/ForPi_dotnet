using System;
using JetBrains.Annotations;

namespace Atom.Common
{
    /// <summary>
    /// Exception extensions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Terminate if the exception is due to OutOfMemory or AccessViolation.
        /// </summary>
        /// <remarks>
        /// Write a message to windows application event log.
        /// </remarks>
        public static Exception FailFastIfCriticalException(
            [CanBeNull] this Exception ex,
            [CanBeNull] string failureMessage)
        {
            if (ex is OutOfMemoryException || ex is AccessViolationException)
            {
                Environment.FailFast(failureMessage);
            }

            return ex;
        }
    }
}