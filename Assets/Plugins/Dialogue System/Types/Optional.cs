using System;

namespace Fields
{
    [Serializable]
    public struct Optional<T>
    {
        public bool valueSet;
        public T value;

        public Optional(T input)
        {
            value = input;
            valueSet = input != null;
        }

        public static implicit operator bool(Optional<T> optional) => optional.valueSet;
        public static implicit operator T(Optional<T> optional) => optional.valueSet ? optional.value : default;
        public static implicit operator Optional<T>(T input) => new(input);
    }

    public static class OptionalExtensions
    {
        public static void Execute(this Optional<Action> action)
        {
            if (action.valueSet)
                action.value();
        }
        
        public static void Execute<T1>(this Optional<Action<T1>> action, T1 arg)
        {
            if (action.valueSet)
                action.value?.Invoke(arg);
        }

    }
}
