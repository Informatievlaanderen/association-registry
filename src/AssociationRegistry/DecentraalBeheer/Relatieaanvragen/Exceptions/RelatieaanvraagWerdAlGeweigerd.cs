namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class RelatieaanvraagWerdAlGeweigerd : DomainException
{
    public RelatieaanvraagWerdAlGeweigerd()
        : base(ExceptionMessages.RelatieaanvraagWerdAlGeweigerd) { }

    protected RelatieaanvraagWerdAlGeweigerd(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
