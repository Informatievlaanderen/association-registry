namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class DoelgroepValtBuitenToegestaneWaarden : DomainException
{
    public DoelgroepValtBuitenToegestaneWaarden() : base(
        $"Minimum en maximum leeftijd moeten tussen {Doelgroep.StandaardMinimumleeftijd} en {Doelgroep.StandaardMaximumleeftijd} inclusief liggen.")
    {
    }

    protected DoelgroepValtBuitenToegestaneWaarden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
