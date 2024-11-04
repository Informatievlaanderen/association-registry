namespace AssociationRegistry.Framework;

public class Throw<TException> where TException : Exception
{
    public static void If(Func<bool> condition, string? message = null)
    {
        if (condition())
            ThrowException(message);
    }

    public static void If(bool invalid, string? message = null)
    {
        if (invalid)
            ThrowException(message);
    }

    public static void IfNot(Func<bool> condition, string? message = null)
    {
        if (!condition())
            ThrowException(message);
    }

    public static void IfNot(bool valid, string? message = null)
    {
        if (!valid)
            ThrowException(message);
    }

    public static void IfNullOrWhiteSpace(string? value, string? message = null)
        => If(string.IsNullOrWhiteSpace(value), message);

    public static void IfNull(object? value, string? message = null)
        => If(value is null, message);

    private static void ThrowException(string? message)
    {
        throw (string.IsNullOrWhiteSpace(message)
            ? Activator.CreateInstance<TException>()
            : Activator.CreateInstance(typeof(TException), message) as TException)!;
    }
}
