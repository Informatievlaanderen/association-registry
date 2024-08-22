namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingWerdVerwijderd : DomainException
{
    public VCode Vcode { get; }

    public VerenigingWerdVerwijderd(VCode vcode) : base(ExceptionMessages.VerenigingWerdVerwijderd)
    {
        Vcode = vcode;
    }

    protected VerenigingWerdVerwijderd(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
