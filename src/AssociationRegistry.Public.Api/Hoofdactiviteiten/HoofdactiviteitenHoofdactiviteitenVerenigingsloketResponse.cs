namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

public class HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse
{
    /// <summary>
    /// Alle hoofdactiviteiten volgens het Verenigingsloket
    /// </summary>
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;

    public class HoofdactiviteitVerenigingsloket
    {
        /// <summary>
        /// De verkorte code van de hoofdactiviteit
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// De volledige beschrijving van de hoofdactiviteit
        /// </summary>
        public string Beschrijving { get; set; } = null!;
    }
}
