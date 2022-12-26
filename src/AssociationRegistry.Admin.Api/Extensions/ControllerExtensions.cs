namespace AssociationRegistry.Admin.Api.Extensions;

using AssociationRegistry.VCodes;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static AcceptedResult AcceptedWithLocation(this ApiController source, VCode vCode)
        => source.Accepted($"/v1/verenigingen/{vCode}");

    public static AcceptedResult AcceptedRegistratie(this ApiController source, RegistratieResult registratieResult)
    {
        source.Response.AddSequenceHeader(registratieResult.Sequence);
        return source.AcceptedWithLocation(registratieResult.Vcode);
    }
}
