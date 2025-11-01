using System;
using System.Collections.Generic;

namespace SurgeEngine.Source.Code.Infrastructure.Custom
{
    public class ReactiveVar<T>
    {
        public event Action<T, T> Changed;

        private T _value;
        private readonly IEqualityComparer<T> _comparer;

        public ReactiveVar() : this(default)
        {
            
        }

        public ReactiveVar(T value) : this(value, EqualityComparer<T>.Default)
        {
            
        }
        
        public ReactiveVar(T value, IEqualityComparer<T> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        public T Value
        {
            get => _value;
            set
            {
                T oldValue = _value;
                _value = value;

                if (!_comparer.Equals(oldValue, value))
                {
                    Changed?.Invoke(oldValue, value);
                }
            }
        }
    }
}