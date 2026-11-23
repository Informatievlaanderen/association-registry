namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The message names the allowed values of <see cref="RelatieType" />.</summary>
[Serializable]
public class RelatieTypeIsOngeldig : DomainException
{
    public RelatieTypeIsOngeldig()
        : base(
            string.Format(
                ExceptionMessages.RelatieTypeIsOngeldig,
                AllowedValues.Describe(RelatieType.All.Select(type => type.Value))
            )
        ) { }

    protected RelatieTypeIsOngeldig(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
