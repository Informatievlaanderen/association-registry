namespace AssociationRegistry.VerenigingsNamen.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class EmptyVerenigingsNaam : Exception
{
    public EmptyVerenigingsNaam() : base("The field 'naam' should have a value.")
    {
    }

    protected EmptyVerenigingsNaam(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
