using System;

namespace NntpClientLib
{
    public sealed class ArticleResponseIds
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleResponseIds"/> class.
        /// </summary>
        private ArticleResponseIds()
        {
        }

        /// <summary>
        /// Gets the article id.
        /// </summary>
        /// <value>The article id.</value>
        public long ArticleId { get; private set; }

        /// <summary>
        /// Gets the message id.
        /// </summary>
        /// <value>The message id.</value>
        public string MessageId { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return ArticleId + " " + MessageId;
        }

        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static ArticleResponseIds Parse(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException("response");
            }
            var a = new ArticleResponseIds();
            var sa = response.Split(new [] {' '});
            if (sa.Length == 2)
            {
                a.ArticleId = Rfc977NntpClient.ConvertArticleId(sa[0]);
                a.MessageId = sa[1];
            }
            else if (sa.Length > 2)
            {
                a.ArticleId = Rfc977NntpClient.ConvertArticleId(sa[1]);
                a.MessageId = sa[2];
            }
            else
            {
                throw new ArgumentException(Resource.ErrorMessage48, "response");
            }
            return a;
        }
    }
}