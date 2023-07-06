namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DoelgroepOutOfRange : DomainException
{
    public DoelgroepOutOfRange() : base($"Minimum en maximum leeftijd moeten tussen {Doelgroep.StandaardMinimumleeftijd} en {Doelgroep.StandaardMaximumleeftijd} inclusief liggen.")
    {
    }

    protected DoelgroepOutOfRange(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
