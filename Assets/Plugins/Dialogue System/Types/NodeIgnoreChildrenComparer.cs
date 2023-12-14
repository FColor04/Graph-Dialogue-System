using System;
using System.Collections.Generic;

namespace Dialogue_System
{
    public class NodeIgnoreChildrenComparer : IEqualityComparer<Input>, IEqualityComparer<Output>
    {
        public bool Equals(Input x, Input y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.label == y.label && x.serializedType == y.serializedType && x.orientation == y.orientation && x.capacity == y.capacity;
        }

        public int GetHashCode(Input obj)
        {
            return HashCode.Combine(obj.label, obj.serializedType, (int)obj.orientation, (int)obj.capacity);
        }

        public bool Equals(Output x, Output y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.label == y.label && x.serializedType == y.serializedType && x.orientation == y.orientation && x.capacity == y.capacity;
        }

        public int GetHashCode(Output obj)
        {
            return HashCode.Combine(obj.label, obj.serializedType, (int)obj.orientation, (int)obj.capacity);
        }
    }
}