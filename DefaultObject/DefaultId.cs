using System;
using DevRating.Domain;

namespace DevRating.DefaultObject
{
    public sealed class DefaultId : Id
    {
        private readonly object _value;

        public DefaultId() : this(DBNull.Value)
        {
        }

        public DefaultId(object value)
        {
            _value = value;
        }

        public object Value()
        {
            return _value;
        }

        public bool Present()
        {
            return !_value.Equals(DBNull.Value);
        }
    }
}