namespace AssociationRegistry.DecentraalBeheer.Vereniging.InStopzetting.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingNietActiefVoorInStopzetting : DomainException
{
    public VerenigingNietActiefVoorInStopzetting()
        : base(ExceptionMessages.VerenigingNietActiefVoorInStopzetting) { }

    protected VerenigingNietActiefVoorInStopzetting(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
