namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultipleCorrespondentieLocaties : DomainException
{
    public MultipleCorrespondentieLocaties() : base("Er kan maar één correspondentie locatie zijn binnen de vereniging.")
    {
    }

    protected MultipleCorrespondentieLocaties(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
