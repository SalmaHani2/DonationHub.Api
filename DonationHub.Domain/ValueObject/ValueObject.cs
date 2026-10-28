using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.ValueObject
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is not ValueObject other)
                return false;

            return GetEqualityComponents()
                .SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(
                    0,
                    (hash, component) =>
                        HashCode.Combine(hash, component));
        }

        public static bool operator ==(
            ValueObject? left,
            ValueObject? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(
            ValueObject? left,
            ValueObject? right)
        {
            return !(left == right);
        }
    }

}
