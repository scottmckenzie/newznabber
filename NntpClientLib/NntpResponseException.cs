using System;

namespace NntpClientLib
{
    public class NntpResponseException : NntpException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NntpResponseException"/> class.
        /// </summary>
        public NntpResponseException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpResponseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NntpResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpResponseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lastResponse">The last response.</param>
        public NntpResponseException(string message, string lastResponse)
            : base(message)
        {
            m_lastResponse = lastResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpResponseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public NntpResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private string m_lastResponse;
        /// <summary>
        /// Gets the last response.
        /// </summary>
        /// <value>The last response.</value>
        public string LastResponse
        {
            get { return m_lastResponse; }
        }

        /// <summary>
        /// Gets the last response code.
        /// </summary>
        /// <value>The last response code.</value>
        public int LastResponseCode
        {
            get { return Convert.ToInt32(m_lastResponse.Substring(0, 3), System.Globalization.CultureInfo.InvariantCulture); }
        }

    }
}

