namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingIsVerwijderd : DomainException
{
    public VCode Vcode { get; }

    public VerenigingIsVerwijderd(VCode vcode) : base(ExceptionMessages.VerenigingIsVerwijderd)
    {
        Vcode = vcode;
    }

    protected VerenigingIsVerwijderd(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
