using System;

namespace NntpClientLib
{
    /// <summary>
    /// 
    /// </summary>
    public class NntpNotAuthorizedException : NntpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpNotAuthorizedException"/> class.
        /// </summary>
        public NntpNotAuthorizedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpNotAuthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NntpNotAuthorizedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpNotAuthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NntpNotAuthorizedException(string message, Exception inner) : base(message, inner) { }

    } 
}

