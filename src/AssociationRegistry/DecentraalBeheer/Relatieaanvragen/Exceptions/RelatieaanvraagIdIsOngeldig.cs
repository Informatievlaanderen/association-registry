namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The message names the format of <see cref="RelatieaanvraagId" />.</summary>
[Serializable]
public class RelatieaanvraagIdIsOngeldig : DomainException
{
    public RelatieaanvraagIdIsOngeldig()
        : base(string.Format(ExceptionMessages.RelatieaanvraagIdIsOngeldig, RelatieaanvraagId.Format)) { }

    protected RelatieaanvraagIdIsOngeldig(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
