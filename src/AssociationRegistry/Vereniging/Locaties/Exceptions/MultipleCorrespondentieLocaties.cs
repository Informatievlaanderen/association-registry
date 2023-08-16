namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultipleCorrespondentieLocaties : DomainException
{
    public MultipleCorrespondentieLocaties() : base(ExceptionMessages.MultipleCorrespondentieLocaties)
    {
    }

    protected MultipleCorrespondentieLocaties(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
