using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace DevRating.Git
{
    internal sealed class FilePatch : Watchdog
    {
        private readonly IRepository _repo;
        private readonly PatchEntryChanges _patch;
        private readonly GitObject _commit;
        private readonly LibGit2Sharp.Commit _parent;
        private readonly string _author;

        public FilePatch(IRepository repo, PatchEntryChanges patch, GitObject commit, LibGit2Sharp.Commit parent,
            string author)
        {
            _repo = repo;
            _patch = patch;
            _commit = commit;
            _parent = parent;
            _author = author;
        }

        public async Task WriteInto(Log log)
        {
            foreach (var hunk in await Task.Run(Hunks))
            {
                await hunk.WriteInto(log);
            }
        }

        private IEnumerable<Hunk> Hunks()
        {
            var blame = _repo.Blame(_patch.OldPath, new BlameOptions
            {
                StartingAt = _parent
            });

            var hunks = new List<Hunk>();
            
            foreach (var line in _patch.Patch.Split('\n'))
            {
                if (line.StartsWith("@@ "))
                {
                    // line must be like "@@ -3,9 +3,9 @@ blah..."
                    var parts = line.Split(' ');

                    hunks.Add(new Hunk(_author, Deletions(_repo, parts[1], blame), Additions(parts[2]), _commit.Sha));
                }
            }

            return hunks;
        }

        private IEnumerable<string> Deletions(IRepository repo, string hunk, BlameHunkCollection blame)
        {
            var deletions = new List<string>();

            var parts = hunk
                .Substring(1)
                .Split(',');

            var index = Convert.ToInt32(parts[0]) - 1;

            var count = parts.Length == 1 ? 1 : Convert.ToInt32(parts[1]);

            for (var i = index; i < index + count; i++)
            {
                deletions.Add(new Author(repo, blame.HunkForLine(i).FinalSignature).Email());
            }

            return deletions;
        }

        private int Additions(string hunk)
        {
            var parts = hunk
                .Substring(1)
                .Split(',');

            var count = parts.Length == 1 ? 1 : Convert.ToInt32(parts[1]);

            return count;
        }
    }
}