namespace AssociationRegistry.Integrations.Magda.Shared.Exceptions;

using AssociationRegistry.Integrations.Magda.Persoon.GeefPersoon;
using AssociationRegistry.Integrations.Magda.Persoon.Models;

public static class MagdaExceptionStringBuilder
{
    public static string Build(UitzonderingType[] magdaFouten)
        => "Magda fouten: " + string.Join("\n\t-", magdaFouten.Select(x => x.Diagnose));

    public static string Build(Repertorium.RegistreerInschrijving0200.UitzonderingType[] magdaFouten)
        => "Magda fouten: " + string.Join("\n\t-", magdaFouten.Select(x => x.Diagnose));

    public static string Build(SoapFault fault)
        => fault.FaultString ?? fault.Detail?.Message ?? NoErrorProvided;

    public const string NoErrorProvided = "Geen foutdetail gekregen van magda";
}
