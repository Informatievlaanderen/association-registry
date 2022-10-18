namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

using System;

public abstract class InvalidVCode : Exception
{
    protected InvalidVCode(string message) : base(message)
    {
    }
}
