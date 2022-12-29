namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using ConfigurationBindings;
using Microsoft.AspNetCore.Mvc;

public static class ControllerExtensions
{
    public static AcceptedResult AcceptedRegistratie(this ApiController source, AppSettings appSettings, RegistratieResult registratieResult)
    {
        source.Response.AddSequenceHeader(registratieResult.Sequence);
        return source.Accepted($"{appSettings.BaseUrl}/v1/verenigingen/{registratieResult.Vcode}");
    }
}
