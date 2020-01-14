using System.Collections.Generic;
using System.Linq;

namespace DevRating.Domain
{
    public sealed class TotalDeletions : Deletions
    {
        private readonly Hunks _hunks;

        public TotalDeletions(Hunks hunks)
        {
            _hunks = hunks;
        }

        public IEnumerable<Deletion> Items()
        {
            return _hunks.Items()
                .SelectMany(HunkDeletions)
                .GroupBy(DeletionAuthor)
                .Select(Deletion);
        }

        private Deletion Deletion(IGrouping<string, Deletion> grouping)
        {
            return new DefaultDeletion(DeletionsAuthor(grouping), DeletionsCountsSum(grouping));
        }

        private IEnumerable<Deletion> HunkDeletions(Hunk hunk)
        {
            return hunk.Deletions().Items();
        }

        private string DeletionAuthor(Deletion deletion)
        {
            return deletion.Email().ToLowerInvariant();
        }

        private string DeletionsAuthor(IGrouping<string, Deletion> grouping)
        {
            return grouping.Key;
        }

        private uint DeletionsCountsSum(IEnumerable<Deletion> grouping)
        {
            return (uint) grouping.Sum(DeletionsCount);
        }

        private long DeletionsCount(Deletion deletion)
        {
            return deletion.Count();
        }
    }
}