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

    public NullOrEmpty<TDestination> ToNullOrEmpty<TDestination>(Func<T, TDestination> transformer)
    {
        return _type switch
        {
            NullOrEmptyType.IsNull => NullOrEmpty<TDestination>.Null,
            NullOrEmptyType.IsEmpty => NullOrEmpty<TDestination>.Empty,
            NullOrEmptyType.HasValue => NullOrEmpty<TDestination>.Create(transformer(Value)),
            _ => throw new ArgumentOutOfRangeException()
        };
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

    public NullOrEmpty<S> Map<S>(Func<T, S> map)
    {
        if (IsNull)
            return NullOrEmpty<S>.Null;

        if (IsEmpty)
            return NullOrEmpty<S>.Empty;

        return NullOrEmpty<S>.Create(map(Value!));
    }

    private enum NullOrEmptyType
    {
        IsNull,
        IsEmpty,
        HasValue,
    }
}
