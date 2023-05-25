namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System;
using Projections.Detail;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    public DetailVerenigingResponse GetExamples()
        => new()
        {
            Vereniging = new DetailVerenigingResponse.VerenigingDetail
            {
                VCode = "V0001001",
                Type = new BeheerVerenigingDetailDocument.VerenigingsType
                {
                    Beschrijving = VerenigingsType.FeitelijkeVereniging.Beschrijving,
                    Code = VerenigingsType.FeitelijkeVereniging.Code,
                },
                Naam = "FWA De vrolijke BA’s",
                KorteNaam = "DVB",
                KorteBeschrijving = "De vereniging van de vrolijke BA's",
                Startdatum = "2020-05-15",
                Status = "Actief",
                HoofdactiviteitenVerenigingsloket = new[]
                {
                    new DetailVerenigingResponse.VerenigingDetail.HoofdactiviteitVerenigingsloket
                    {
                        Code = "CULT",
                        Beschrijving = "Cultuur",
                    },
                },
                Contactgegevens = new[]
                {
                    new DetailVerenigingResponse.VerenigingDetail.Contactgegeven
                    {
                        Type = "Email",
                        Beschrijving = "Info",
                        Waarde = "info@example.org",
                        ContactgegevenId = 1,
                        IsPrimair = false,
                    },
                },
                Locaties = new[]
                {
                    new DetailVerenigingResponse.VerenigingDetail.Locatie
                    {
                        Locatietype = "Correspondentie",
                        Hoofdlocatie = true,
                        Adres = "kerkstraat 5, 1770 Liedekerke, Belgie",
                        Naam = null,
                        Postcode = "1770",
                        Gemeente = "Liedekerke",
                    },
                },
                Vertegenwoordigers = new[]
                {
                    new DetailVerenigingResponse.VerenigingDetail.Vertegenwoordiger
                    {
                        Insz = "xx.xx.xx-xxx-xx",
                        VertegenwoordigerId = 1,
                        Voornaam = "Jhon",
                        Achternaam = "Doo",
                        PrimairContactpersoon = false,
                        Roepnaam = "Jhony",
                        Rol = "Voorzitter",
                        Email = "jhon@example.org",
                        Mobiel = "0000112233",
                        Telefoon = "0001112233",
                        SocialMedia = "http://example.org",
                    },
                },
                Sleutels = Array.Empty<DetailVerenigingResponse.VerenigingDetail.Sleutel>(),
            },
            Metadata = new DetailVerenigingResponse.MetadataDetail
            {
                DatumLaatsteAanpassing = "2020-05-15",
                BeheerBasisUri = "/verenigingen/V0001001",
            },
        };
}
