using System;

namespace NntpClientLib
{
    [Serializable]
    public sealed class NewsgroupStatistics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewsgroupStatistics"/> class.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="estimateCount">The estimate article count.</param>
        /// <param name="firstArticleId">The first article id.</param>
        /// <param name="lastArticleId">The last article id.</param>
        public NewsgroupStatistics(string groupName, int estimateCount, long firstArticleId, long lastArticleId)
        {
            GroupName = groupName;
            EstimatedCount = estimateCount;
            FirstArticleId = firstArticleId;
            LastArticleId = lastArticleId;
        }

        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; private set; }

        /// <summary>
        /// Gets the estimated article count.
        /// </summary>
        /// <value>The estimated count.</value>
        public int EstimatedCount { get; private set; }

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
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return GroupName + " " + EstimatedCount + " " + FirstArticleId + " " + LastArticleId; 
        }
    }
}

