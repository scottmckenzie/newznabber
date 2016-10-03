using System;

namespace NntpClientLib
{
    public class NntpGroupNotSelectedException : NntpException
    {
        public NntpGroupNotSelectedException() { }
        public NntpGroupNotSelectedException(string message) : base(message) { }
        public NntpGroupNotSelectedException(string message, Exception inner) : base(message, inner) { }
    }
}

