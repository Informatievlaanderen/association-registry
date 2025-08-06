namespace AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;

using DecentraalBeheer.Vereniging;
using GemeentenaamVerrijking;
using System.Text.RegularExpressions;
using Vereniging;

public class RemoveGemeentenaamFromVerenigingsNaam
{
    public static string Remove(VerenigingsNaam naam, VerrijkteGemeentenaam[] gemeentes)
    {
        var naamZonderGemeentes = naam.ToString().ToLower();

        foreach (var gemeente in gemeentes.SelectMany(x => new[]{x.Gemeentenaam.ToLower(), x.Postnaam?.Value.ToLower()}).Where(y => y != null))
        {
            var pattern = @$"(?<=[\p{{P}}]){gemeente}(?=[\p{{P}}])";
            naamZonderGemeentes = naamZonderGemeentes.Replace($" {gemeente} ", " ");
            naamZonderGemeentes = Regex.Replace(naamZonderGemeentes, pattern, "").Trim();

            if (naamZonderGemeentes.StartsWith($"{gemeente} "))
            {
                naamZonderGemeentes = naamZonderGemeentes.Replace($"{gemeente} ", string.Empty);
            }
            if (naamZonderGemeentes.EndsWith($" {gemeente}"))
            {
                naamZonderGemeentes = naamZonderGemeentes.Replace($" {gemeente}", string.Empty);
            }
        }

        return naamZonderGemeentes;
    }
}
