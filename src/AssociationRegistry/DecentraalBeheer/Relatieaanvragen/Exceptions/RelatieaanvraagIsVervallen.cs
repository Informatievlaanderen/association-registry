namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>The vervaldatum is over, also when the status is not Vervallen yet.</summary>
[Serializable]
public class RelatieaanvraagIsVervallen : DomainException
{
    public RelatieaanvraagIsVervallen()
        : base(ExceptionMessages.RelatieaanvraagIsVervallen) { }

    protected RelatieaanvraagIsVervallen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
