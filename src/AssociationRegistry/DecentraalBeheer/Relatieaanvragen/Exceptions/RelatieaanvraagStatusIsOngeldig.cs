namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The message names the allowed values of <see cref="RelatieaanvraagStatus" />.</summary>
[Serializable]
public class RelatieaanvraagStatusIsOngeldig : DomainException
{
    public RelatieaanvraagStatusIsOngeldig()
        : base(
            string.Format(
                ExceptionMessages.RelatieaanvraagStatusIsOngeldig,
                AllowedValues.Describe(RelatieaanvraagStatus.All.Select(status => status.Value))
            )
        ) { }

    protected RelatieaanvraagStatusIsOngeldig(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
