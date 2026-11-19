namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using DomainKoepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

public class WijzigKoepelgegevensRequestExamples : IExamplesProvider<WijzigKoepelgegevensRequest>
{
    public WijzigKoepelgegevensRequest GetExamples() =>
        new() { Koepelstatus = DomainKoepelstatus.Koepel.Value, PrivacyVerklaringGeaccepteerd = true };
}
