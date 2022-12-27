namespace AssociationRegistry.Public.Api;

using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Hosting;

public class Program
{
    protected Program()
    { }

    public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();


    private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        => new WebHostBuilder()
            .UseDefaultForApi<Startup>(
                new ProgramOptions
                {
                    Hosting =
                    {
                        HttpPort = 11003,
                    },
                    Logging =
                    {
                        WriteTextToConsole = false,
                        WriteJsonToConsole = false,
                    },
                    Runtime =
                    {
                        CommandLineArgs = args,
                    },
                });
}
