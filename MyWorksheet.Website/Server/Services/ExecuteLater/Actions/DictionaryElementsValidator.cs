using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.Actions;

public class DictionaryElementsValidator<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _dictionary;

    public DictionaryElementsValidator(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
        Errors = [];
        Result = true;
    }

    public bool Result { get; set; }
    public ICollection<string> Errors { get; set; }

    private void SetResult(Func<bool> condition, Func<string> errText)
    {
        try
        {
            if (condition())
            {
                return;
            }

            Result = false;
            Errors.Add(errText());
        }
        catch (Exception e)
        {
            Result = false;
            Errors.Add(e.ToString());
        }
    }

    public DictionaryElementsValidator<TKey, TValue> ContainsKey(TKey key)
    {
        SetResult(() => _dictionary.ContainsKey(key), () => $"Key: '{key}' does not exist");
        return this;
    }

    public DictionaryElementsValidator<TKey, TValue> OfType<T>(TKey key)
    {
        SetResult(() => _dictionary[key].GetType() == typeof(T), () => $"Key: '{key}' is not of type '{typeof(T).Name}'");
        return this;
    }

    public DictionaryElementsValidator<TKey, TValue> Is<T>(TKey key)
    {
        SetResult(() => _dictionary[key] is T, () => $"Key: '{key}' is not of type '{typeof(T).Name}'");
        return this;
    }
}