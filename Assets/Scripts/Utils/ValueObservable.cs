using System;
using System.Collections.Generic;

public class ValueObservable<T> {
    
    private Action observers;
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            observers?.Invoke();
        }
    }

    public ValueObservable(T value)
    {
        _value = value;
    }

    public void Subscribe(Action onChange)
    {
        observers += onChange;
    }
}
