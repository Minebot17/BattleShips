using System;
using UnityEngine;

public class PrefsValue<T>
{
    private string key;
    private bool isLoaded;
    private T defaultValue;
    private ValueObservable<T> _value;
    
    public T Value
    {
        get
        {
            if (!isLoaded)
            {
                isLoaded = true;
                _value.Value = ValueGet();
            }

            return _value.Value;
        }

        set
        {
            _value.Value = value;
            ValueSet(value);
        }
    }

    public PrefsValue(string key, T defaultValue)
    {
        this.key = key;
        this.defaultValue = defaultValue;
        _value = new ValueObservable<T>(defaultValue);
    }

    public void Subscribe(Action onChange)
    {
        _value.Subscribe(onChange);
    }

    private void ValueSet(T value)
    {
        switch (value)
        {
            case int i:
                PlayerPrefs.SetInt(key, i);
                break;
            case float f:
                PlayerPrefs.SetFloat(key, f);
                break;
            case string s:
                PlayerPrefs.SetString(key, s);
                break;
        }
    }
    
    private T ValueGet()
    {
        if (typeof(T) == typeof(int))
            return (T)(object) PlayerPrefs.GetInt(key, (int)(object) defaultValue);
        if (typeof(T) == typeof(float))
            return (T)(object) PlayerPrefs.GetFloat(key, (float)(object) defaultValue);
        if (typeof(T) == typeof(string))
            return (T) (object) PlayerPrefs.GetString(key, (string)(object) defaultValue);
        
        return _value.Value;
    }
}
