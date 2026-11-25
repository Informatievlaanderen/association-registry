namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class RelatieaanvraagWerdAlIngetrokken : DomainException
{
    public RelatieaanvraagWerdAlIngetrokken()
        : base(ExceptionMessages.RelatieaanvraagWerdAlIngetrokken) { }

    protected RelatieaanvraagWerdAlIngetrokken(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
