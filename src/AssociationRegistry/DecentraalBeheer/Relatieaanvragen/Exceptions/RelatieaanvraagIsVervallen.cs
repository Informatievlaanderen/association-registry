namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>
///     The status is Vervallen, or the vervaldatum is over and the job of the scheduled host did not close it yet.
/// </summary>
[Serializable]
public class RelatieaanvraagIsVervallen : DomainException
{
    public RelatieaanvraagIsVervallen()
        : base(ExceptionMessages.RelatieaanvraagIsVervallen) { }

    protected RelatieaanvraagIsVervallen(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
