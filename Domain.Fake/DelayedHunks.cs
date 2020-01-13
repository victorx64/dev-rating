using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevRating.Domain.Fake
{
    public sealed class DelayedHunks : Hunks
    {
        private readonly Hunks _origin;
        private readonly TimeSpan _delay;

        public DelayedHunks(Hunks origin, TimeSpan delay)
        {
            _origin = origin;
            _delay = delay;
        }

        public IEnumerable<Hunk> Items()
        {
            Task.Delay(_delay).ConfigureAwait(false).GetAwaiter().GetResult();

            return _origin.Items();
        }
    }
}