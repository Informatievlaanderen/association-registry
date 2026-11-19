namespace AssociationRegistry.DecentraalBeheer.Vereniging.Koepels.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn : DomainException
{
    public EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn()
        : base(ExceptionMessages.EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn) { }

    protected EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
