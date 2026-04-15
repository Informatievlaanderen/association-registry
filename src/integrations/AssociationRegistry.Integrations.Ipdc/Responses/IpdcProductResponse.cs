namespace AssociationRegistry.Integrations.Ipdc.Responses;

using System.Text.Json.Serialization;

public class IpdcProductResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("@id")]
    public string AtId { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("verdereBeschrijving")]
    public Translation VerdereBeschrijving { get; set; }

    [JsonPropertyName("bevoegdBestuursniveaus")]
    public List<string> BevoegdBestuursniveaus { get; set; }

    [JsonPropertyName("uitvoerendBestuursniveaus")]
    public List<string> UitvoerendBestuursniveaus { get; set; }

    [JsonPropertyName("startDienstVerlening")]
    public DateTime? StartDienstVerlening { get; set; }

    [JsonPropertyName("eindeDienstVerlening")]
    public DateTime? EindeDienstVerlening { get; set; }

    [JsonPropertyName("publicatiekanalen")]
    public List<string> Publicatiekanalen { get; set; }

    [JsonPropertyName("cases")]
    public List<Case> Cases { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }

    [JsonPropertyName("themas")]
    public List<string> Themas { get; set; }

    [JsonPropertyName("doelgroepen")]
    public List<string> Doelgroepen { get; set; }

    [JsonPropertyName("talen")]
    public List<string> Talen { get; set; }

    [JsonPropertyName("geografischToepassingsgebieden")]
    public List<string> GeografischToepassingsgebieden { get; set; }

    [JsonPropertyName("yourEuropeCategorieeen")]
    public List<string> YourEuropeCategorieeen { get; set; }

    [JsonPropertyName("regelgevingTekst")]
    public Translation RegelgevingTekst { get; set; }

    [JsonPropertyName("uitzonderingen")]
    public Translation Uitzonderingen { get; set; }

    [JsonPropertyName("aangemaaktDoor")]
    public OverheidReference AangemaaktDoor { get; set; }

    [JsonPropertyName("zoektermen")]
    public Translation Zoektermen { get; set; }

    [JsonPropertyName("bevoegdeOverheden")]
    public List<OverheidReference> BevoegdeOverheden { get; set; }

    [JsonPropertyName("uitvoerendeOverheden")]
    public List<OverheidReference> UitvoerendeOverheden { get; set; }

    [JsonPropertyName("contactOverheden")]
    public List<OverheidReference> ContactOverheden { get; set; }

    [JsonPropertyName("contactBeschrijving")]
    public Translation ContactBeschrijving { get; set; }

    [JsonPropertyName("socialeKaartOrganisaties")]
    public List<SocialeKaartOrganisatie> SocialeKaartOrganisaties { get; set; }

    [JsonPropertyName("voorwaarden")]
    public List<Voorwaarde> Voorwaarden { get; set; }

    [JsonPropertyName("bewijs")]
    public Bewijs Bewijs { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }

    [JsonPropertyName("procedures")]
    public List<Procedure> Procedures { get; set; }

    [JsonPropertyName("websites")]
    public List<Website> Websites { get; set; }

    [JsonPropertyName("kosten")]
    public List<NamedItem> Kosten { get; set; }

    [JsonPropertyName("financieleVoordelen")]
    public List<NamedItem> FinancieleVoordelen { get; set; }

    [JsonPropertyName("regelgeving")]
    public List<Regelgeving> Regelgeving { get; set; }

    [JsonPropertyName("contactgegevens")]
    public List<Contactgegeven> Contactgegevens { get; set; }

    [JsonPropertyName("creatie")]
    public DateTime? Creatie { get; set; }

    [JsonPropertyName("laatstGewijzigd")]
    public DateTime? LaatstGewijzigd { get; set; }

    [JsonPropertyName("laatstOntvangen")]
    public DateTime? LaatstOntvangen { get; set; }

    [JsonPropertyName("linkedConcept")]
    public string LinkedConcept { get; set; }

    [JsonPropertyName("linkedConceptId")]
    public string LinkedConceptId { get; set; }

    [JsonPropertyName("linkedConceptProductnummer")]
    public string LinkedConceptProductnummer { get; set; }

    [JsonPropertyName("productnummer")]
    public string Productnummer { get; set; }

    [JsonPropertyName("gearchiveerd")]
    public bool Gearchiveerd { get; set; }

    [JsonPropertyName("uitgesloten")]
    public bool Uitgesloten { get; set; }

    [JsonPropertyName("machineLeesbareVoorwaarden")]
    public List<MachineLeesbareVoorwaarde> MachineLeesbareVoorwaarden { get; set; }

    [JsonPropertyName("publicatieLink")]
    public Website PublicatieLink { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("verwanteProducten")]
    public List<VerwantProduct> VerwanteProducten { get; set; }

    [JsonPropertyName("deminimis")]
    public bool Deminimis { get; set; }
}

public class Translation
{
    [JsonPropertyName("nl")]
    public string Nl { get; set; }

    [JsonPropertyName("en")]
    public string En { get; set; }
}

public class OverheidReference
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("@id")]
    public string AtId { get; set; }
}

public class Case
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class SocialeKaartOrganisatie
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class Voorwaarde
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }
}

public class Bewijs
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }
}

public class Procedure
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("websites")]
    public List<Website> Websites { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

public class Website
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

public class NamedItem
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

public class Regelgeving
{
    [JsonPropertyName("naam")]
    public Translation Naam { get; set; }

    [JsonPropertyName("beschrijving")]
    public Translation Beschrijving { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

public class Contactgegeven
{
    [JsonPropertyName("adres")]
    public Adres Adres { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("telefoonnummer")]
    public string Telefoonnummer { get; set; }

    [JsonPropertyName("openingsuren")]
    public string Openingsuren { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}

public class Adres
{
    [JsonPropertyName("land")]
    public Translation Land { get; set; }

    [JsonPropertyName("huisnummer")]
    public string Huisnummer { get; set; }

    [JsonPropertyName("busnummer")]
    public string Busnummer { get; set; }

    [JsonPropertyName("postcode")]
    public string Postcode { get; set; }

    [JsonPropertyName("gemeentenaam")]
    public Translation Gemeentenaam { get; set; }

    [JsonPropertyName("straatnaam")]
    public Translation Straatnaam { get; set; }

    [JsonPropertyName("@type")]
    public string AtType { get; set; }
}

public class MachineLeesbareVoorwaarde
{
    [JsonPropertyName("voorwaarden")]
    public List<object> Voorwaarden { get; set; }

    [JsonPropertyName("subsidiemaatregel")]
    public string Subsidiemaatregel { get; set; }
}

public class Metadata
{
    [JsonPropertyName("uJeVersie")]
    public string UJeVersie { get; set; }

    [JsonPropertyName("generatedFormeleFields")]
    public bool GeneratedFormeleFields { get; set; }

    [JsonPropertyName("generatedInformeleFields")]
    public bool GeneratedInformeleFields { get; set; }
}

public class VerwantProduct
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("productnummer")]
    public int? Productnummer { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("order")]
    public int? Order { get; set; }
}
