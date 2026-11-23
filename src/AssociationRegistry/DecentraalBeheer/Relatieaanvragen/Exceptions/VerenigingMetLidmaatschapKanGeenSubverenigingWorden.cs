namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingMetLidmaatschapKanGeenSubverenigingWorden : DomainException
{
    public VerenigingMetLidmaatschapKanGeenSubverenigingWorden()
        : base(ExceptionMessages.VerenigingMetLidmaatschapKanGeenSubverenigingWorden) { }

    protected VerenigingMetLidmaatschapKanGeenSubverenigingWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
