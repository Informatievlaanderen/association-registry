namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden : DomainException
{
    public VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden(RelatieaanvraagId aanvraagId)
        : base(
            string.Format(ExceptionMessages.VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden, aanvraagId.Value)
        ) { }

    protected VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
