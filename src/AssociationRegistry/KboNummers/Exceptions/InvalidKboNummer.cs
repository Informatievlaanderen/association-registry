namespace AssociationRegistry.KboNummers.Exceptions;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public abstract class InvalidKboNummer : DomainException
{
    protected InvalidKboNummer(string message) : base(message)
    {
    }
}
