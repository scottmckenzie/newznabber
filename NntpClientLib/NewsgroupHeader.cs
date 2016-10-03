using System;

namespace NntpClientLib
{
    public sealed class NewsgroupHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsgroupHeader"/> class.
        /// </summary>
        private NewsgroupHeader()
        {
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; private set; }

        /// <summary>
        /// Gets the first article id.
        /// </summary>
        /// <value>The first article id.</value>
        public long FirstArticleId { get; private set; }

        /// <summary>
        /// Gets the last article id.
        /// </summary>
        /// <value>The last article id.</value>
        public long LastArticleId { get; private set; }

        /// <summary>
        /// Gets the status code. This will be indicate whether posting is allowed to this group ("y") or not ("n")
        /// or postings will be forwarded to the newsgroup moderator ("m").
        /// </summary>
        /// <value>The status code.</value>
        public char StatusCode { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return GroupName + " " + FirstArticleId + " " + LastArticleId + " " + StatusCode;
        }

        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static NewsgroupHeader Parse(string response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var parts = response.Split(new [] {' '});
            if (parts.Length < 3)
            {
                throw new ArgumentException(Resource.ErrorMessage13);
            }

            var h = new NewsgroupHeader();
            h.GroupName = parts[0];
            h.LastArticleId = Rfc977NntpClient.ConvertArticleId(parts[1]);
            h.FirstArticleId = Rfc977NntpClient.ConvertArticleId(parts[2]);
            h.StatusCode = ((parts.Length > 3 && parts[3].Length > 0) ? h.StatusCode = parts[3][0] : 'y');
            return h;
        }
    }
}