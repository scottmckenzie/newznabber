using System;
using System.Collections.Generic;

namespace NntpClientLib
{
    public class ArticleHeadersDictionary : Dictionary<string, List<string>>, IArticleHeadersProcessor
    {
        private string _lastHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleHeadersDictionary"/> class.
        /// </summary>
        public ArticleHeadersDictionary()
        {
        }

        #region IArticleHeadersProcessor Members

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="headerAndValue">The header and value with a ":" separator.</param>
        public void AddHeader(string headerAndValue)
        {
            if (headerAndValue == null)
            {
                throw new ArgumentNullException("headerAndValue");
            }

            int idx = headerAndValue.IndexOf(": ");
            if (idx == -1)
            {
                if (string.IsNullOrEmpty(_lastHeader))
                {
                    throw new NntpException();
                }
                if (headerAndValue.StartsWith(" ") || headerAndValue.StartsWith("\t"))
                {
                    AddHeader(_lastHeader, headerAndValue.TrimStart());
                    return;
                }
            }
            int idxOfValue = idx + 2;
            string key = headerAndValue.Substring(0, idx);
            AddHeader(key, headerAndValue.Substring(idxOfValue));
        }

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="value">The value.</param>
        public void AddHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(header))
            {
                throw new ArgumentException("The header value must not be null or an empty string.", "header");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!ContainsKey(header))
            {
                Add(header, new List<string>());
            }
            base[header].Add(value);
            _lastHeader = header;
        }

        #endregion
    }
}

