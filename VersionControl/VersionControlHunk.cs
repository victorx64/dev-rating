// Copyright (c) 2019-present Viktor Semenov
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DevRating.VersionControl
{
    public sealed class VersionControlHunk : Hunk
    {
        private readonly Deletions _deletions;
        private readonly Additions _additions;

        public VersionControlHunk(string patch, Blames blames)
            : this(new VersionControlDeletions(patch, blames), new VersionControlAdditions(patch))
        {
        }

        public VersionControlHunk(Deletions deletions, Additions additions)
        {
            _deletions = deletions;
            _additions = additions;
        }

        public Deletions Deletions()
        {
            return _deletions;
        }

        public Additions Additions()
        {
            return _additions;
        }
    }
}