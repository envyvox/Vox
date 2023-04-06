using System;

namespace Vox.Services.Discord.Client.Extensions
{
    public class ExceptionExtensions
    {
        /// <summary>
        /// An expected error that is thrown only through the fault of the user.
        /// It is displayed back to the user and is not logged in any way.
        /// </summary>
        public class ExpectedException : Exception
        {
            public ExpectedException()
            {
            }

            public ExpectedException(string message)
                : base(message)
            {
            }

            public ExpectedException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
    }
}
