using System;
using System.Collections.Generic;
using System.Text;

namespace NntpClientLib
{
    public class ArticleHeadersDictionaryEnumerator : IArticleHeaderEnumerator
    {
        private readonly Dictionary<string, List<string>> _dict;

        /// <summary>
        /// Gets the header keys.
        /// </summary>
        /// <value>The header keys.</value>
        public IEnumerable<string> HeaderKeys
        {
            get { return _dict.Keys; }
        }

        /// <summary>
        /// Gets the <see cref="string"/> with the specified header key.
        /// Each named header can potentially have multiple values, so we manage this with a list.
        /// </summary>
        /// <value></value>
        public IList<string> this[string headerKey]
        {
            get { return _dict[headerKey]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleHeadersDictionaryEnumerator"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ArticleHeadersDictionaryEnumerator(Dictionary<string, List<string>> dictionary)
        {
            _dict = dictionary;
        }
    }
}