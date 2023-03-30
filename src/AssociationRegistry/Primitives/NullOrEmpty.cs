namespace AssociationRegistry.Primitives;

public readonly struct NullOrEmpty<T>
{
    public T? Value { get; }
    private readonly NullOrEmptyType _type;

    public NullOrEmpty()
    {
        Value = default;
        _type = NullOrEmptyType.IsNull;
    }

    private NullOrEmpty(T? value, NullOrEmptyType type)
    {
        Value = value;
        _type = type;
    }

    public bool IsEmpty
        => _type == NullOrEmptyType.IsEmpty;

    public bool IsNull
        => _type == NullOrEmptyType.IsNull;

    public bool HasValue
        => _type == NullOrEmptyType.HasValue;

    public static NullOrEmpty<T> Create(T? value)
        => value is null ? Null : new NullOrEmpty<T>(value, NullOrEmptyType.HasValue);

    public static NullOrEmpty<T> Empty
        => new(value: default, NullOrEmptyType.IsEmpty);

    public static NullOrEmpty<T> Null
        => new(value: default, NullOrEmptyType.IsNull);

    private enum NullOrEmptyType
    {
        IsNull,
        IsEmpty,
        HasValue,
    }
}
