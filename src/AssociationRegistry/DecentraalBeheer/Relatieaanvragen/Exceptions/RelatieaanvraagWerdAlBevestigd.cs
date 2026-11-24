namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class RelatieaanvraagWerdAlBevestigd : DomainException
{
    public RelatieaanvraagWerdAlBevestigd()
        : base(ExceptionMessages.RelatieaanvraagWerdAlBevestigd) { }

    protected RelatieaanvraagWerdAlBevestigd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
