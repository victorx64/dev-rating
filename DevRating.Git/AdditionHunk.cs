using System.Collections.Generic;

namespace DevRating.Git
{
    public class AdditionHunk : Hunk, IAdditionHunk
    {
        public AdditionHunk(string author, string header) : base(author, header)
        {
        }

        public IList<string> AddTo(IEnumerable<string> authors)
        {
            var output = new List<string>(authors);

            for (var i = 0; i < Count; i++)
            {
                output.Insert(Index, Author);
            }

            return output;
        }
    }
}