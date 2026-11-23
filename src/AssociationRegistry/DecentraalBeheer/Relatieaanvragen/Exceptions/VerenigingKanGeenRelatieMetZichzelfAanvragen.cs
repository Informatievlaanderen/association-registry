namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class VerenigingKanGeenRelatieMetZichzelfAanvragen : DomainException
{
    public VerenigingKanGeenRelatieMetZichzelfAanvragen()
        : base(ExceptionMessages.VerenigingKanGeenRelatieMetZichzelfAanvragen) { }

    protected VerenigingKanGeenRelatieMetZichzelfAanvragen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
