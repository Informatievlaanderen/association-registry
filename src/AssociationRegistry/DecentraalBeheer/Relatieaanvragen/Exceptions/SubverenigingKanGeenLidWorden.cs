namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class SubverenigingKanGeenLidWorden : DomainException
{
    public SubverenigingKanGeenLidWorden()
        : base(ExceptionMessages.SubverenigingKanGeenLidWorden) { }

    protected SubverenigingKanGeenLidWorden(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
