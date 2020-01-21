using System;
using Xunit;

namespace DevRating.DefaultObject.Test
{
    public sealed class DefaultIdTest
    {
        [Fact]
        public void ReturnsValueFromCtor()
        {
            var rating = 1d;

            Assert.Equal(rating, new DefaultId(rating).Value());
        }

        [Fact]
        public void ReturnsNullValueByDefault()
        {
            Assert.Equal(DBNull.Value, new DefaultEnvelope().Value());
        }

        [Fact]
        public void IsNotPresentByDefault()
        {
            Assert.False(new DefaultId().Present());
        }
    }
}