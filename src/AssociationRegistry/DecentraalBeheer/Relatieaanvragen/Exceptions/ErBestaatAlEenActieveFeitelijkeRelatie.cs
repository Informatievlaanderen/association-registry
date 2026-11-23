namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErBestaatAlEenActieveFeitelijkeRelatie : DomainException
{
    public ErBestaatAlEenActieveFeitelijkeRelatie()
        : base(ExceptionMessages.ErBestaatAlEenActieveFeitelijkeRelatie) { }

    protected ErBestaatAlEenActieveFeitelijkeRelatie(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
