using System;
using DevRating.Domain;

namespace DevRating.DefaultObject
{
    public sealed class NullObjectEnvelope : ObjectEnvelope
    {
        public object Value()
        {
            return DBNull.Value;
        }
    }
}