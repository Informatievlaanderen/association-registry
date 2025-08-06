namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingIsDubbel : DomainException
{
    public VCode Vcode { get; }

    public VerenigingIsDubbel(VCode vcode) : base(ExceptionMessages.VerenigingIsDubbel)
    {
        Vcode = vcode;
    }

    protected VerenigingIsDubbel(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
