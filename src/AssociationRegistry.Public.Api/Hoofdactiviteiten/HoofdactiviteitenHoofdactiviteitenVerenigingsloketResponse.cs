﻿namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

public record HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse(HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket)
{
    public record HoofdactiviteitVerenigingsloket(string Code, string Beschrijving);
};
