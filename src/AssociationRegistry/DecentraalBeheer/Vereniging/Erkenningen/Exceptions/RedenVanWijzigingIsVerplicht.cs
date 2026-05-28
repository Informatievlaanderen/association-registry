namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class RedenVanWijzigingIsVerplicht : DomainException
{
    public RedenVanWijzigingIsVerplicht()
        : base(ExceptionMessages.RedenVanWijzigingIsVerplicht) { }

    protected RedenVanWijzigingIsVerplicht(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
