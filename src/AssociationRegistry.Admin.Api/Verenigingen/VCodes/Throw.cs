namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using System;

public class Throw<TException> where TException : Exception, new()
{
    public static void If(Func<bool> condition)
    {
        if (condition())
            throw new TException();
    }

    public static void If(bool invalid)
    {
        if (invalid)
            throw new TException();
    }

    public static void IfNot(Func<bool> condition)
    {
        if (!condition())
            throw new TException();
    }

    public static void IfNot(bool valid)
    {
        if (!valid)
            throw new TException();
    }
}