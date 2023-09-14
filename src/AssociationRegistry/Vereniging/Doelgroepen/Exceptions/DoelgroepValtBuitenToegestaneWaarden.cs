namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DoelgroepValtBuitenToegestaneWaarden : DomainException
{
    public DoelgroepValtBuitenToegestaneWaarden() : base($"Minimum en maximum leeftijd moeten tussen {Doelgroep.StandaardMinimumleeftijd} en {Doelgroep.StandaardMaximumleeftijd} inclusief liggen.")
    {
    }

    protected DoelgroepValtBuitenToegestaneWaarden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
