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
        => new(default, NullOrEmptyType.IsEmpty);

    public static NullOrEmpty<T> Null
        => new(default, NullOrEmptyType.IsNull);

    public static implicit operator T?(NullOrEmpty<T> nullOrEmpty)
        => nullOrEmpty.Value;

    public static implicit operator NullOrEmpty<T>(T? value)
        => Create(value);
    private enum NullOrEmptyType
    {
        IsNull,
        IsEmpty,
        HasValue,
    }
}
