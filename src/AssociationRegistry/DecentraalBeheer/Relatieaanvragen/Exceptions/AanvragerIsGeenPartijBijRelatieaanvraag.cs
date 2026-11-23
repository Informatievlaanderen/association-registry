namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The vereniging in vr-namens must be the koepel or the vereniging of the relatieaanvraag.</summary>
[Serializable]
public class AanvragerIsGeenPartijBijRelatieaanvraag : DomainException
{
    public AanvragerIsGeenPartijBijRelatieaanvraag()
        : base(ExceptionMessages.AanvragerIsGeenPartijBijRelatieaanvraag) { }

    protected AanvragerIsGeenPartijBijRelatieaanvraag(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
