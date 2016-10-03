using System;

namespace NntpClientLib
{
    /// <summary>
    /// 
    /// </summary>
    public class NntpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        public NntpException() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NntpException(string message) : base(message) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NntpException(string message, Exception inner) : base(message, inner) { }

    }
}

