namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

public class OutOfRangeVCode : InvalidVCode
{
    public OutOfRangeVCode() : base("VCode must be between 1 and 999999 (inclusive)")
    {
    }
}