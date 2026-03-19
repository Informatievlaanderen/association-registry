namespace AssociationRegistry.Test.Bewaartermijnen.Verlopen.Commandhandling;

using Admin.Schema.Bewaartermijn;
using Common.Framework;
using Xunit;

public class Given_An_Existing_Vertegenwoordiger
{
    public Given_An_Existing_Vertegenwoordiger()
    {
        var store =  TestDocumentStoreFactory.CreateAsync(nameof(Given_An_Existing_Vertegenwoordiger)).GetAwaiter().GetResult();

    }

    [Fact]
    public void Then_Remove_Vertegenwoordiger_Persoonsgegevens()
    {

    }
}
