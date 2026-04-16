using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Util;

namespace MyWorksheet.Website.Client.Services.LocalStorage;

public class StorageEntry<TValue>
    where TValue : class
{
    public string Name { get; }
    private readonly StorageService _storageService;
    private readonly TValue _defaultValue;

    public StorageEntry(StorageService storageService, string name) : this(storageService, name, default)
    {

    }

    public StorageEntry(StorageService storageService, string name, TValue defaultValue)
    {
        Name = name;
        _storageService = storageService;
        _defaultValue = defaultValue;
        Value = default;
        ValueChanged = new PubSubEvent<TValue>();
    }

    private KeyValuePair<TValue, bool> Value { get; set; }

    public PubSubEvent<TValue> ValueChanged { get; set; }

    public async ValueTask WriteValue(TValue value)
    {
        var store = _storageService.LocalStorageFactory();
        if (store == null)
        {
            return;
        }

        Value = new KeyValuePair<TValue, bool>(value, true);
        await ValueChanged.Raise(value ?? _defaultValue);
        if (value == null)
        {
            await store.RemoveItemAsync(Name);
        }
        else
        {
            await store.SetItemAsync(Name, value);
        }
    }

    public async ValueTask<TValue> ReadValue()
    {
        if (Value.Value)
        {
            return Value.Key ?? _defaultValue;
        }

        var store = _storageService.LocalStorageFactory();
        if (store == null)
        {
            return _defaultValue;
        }

        var value = await store.GetItemAsync<TValue>(Name);
        Value = new KeyValuePair<TValue, bool>(value, true);
        await ValueChanged.Raise(value ?? _defaultValue);
        return value ?? _defaultValue;
    }
}