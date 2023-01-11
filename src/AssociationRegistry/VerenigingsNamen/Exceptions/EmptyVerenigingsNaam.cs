namespace AssociationRegistry.VerenigingsNamen.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class EmptyVerenigingsNaam : Exception
{
    public EmptyVerenigingsNaam() : base("De naam van de vereniging is verplicht.")
    {
    }

    protected EmptyVerenigingsNaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
