namespace AssociationRegistry.KboNummers.Exceptions;

using System;

public abstract class InvalidKboNummer : Exception
{
    protected InvalidKboNummer(string message) : base(message)
    {
    }
}
