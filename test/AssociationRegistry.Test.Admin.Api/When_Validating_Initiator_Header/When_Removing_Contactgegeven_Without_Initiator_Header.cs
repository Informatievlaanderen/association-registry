namespace AssociationRegistry.Test.Admin.Api.When_Validating_Initiator_Header;

using System.Net;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Framework;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class Delete_Without_Initiator_Header : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _eventFixture;
    private readonly Fixture _fixture;
    public HttpResponseMessage Response { get; private set; } = null!;


    public Delete_Without_Initiator_Header(EventsInDbScenariosFixture eventFixture)
    {
        _eventFixture = eventFixture;
        _fixture = new Fixture().CustomizeAll();
    }

    public async Task InitializeAsync()
    {
        Response = await _eventFixture.AdminApiClient.DeleteContactgegeven(
            _fixture.Create<VCode>(),
            _fixture.Create<int>(),
            initiator: null);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class When_Removing_Contactgegeven_Without_Initiator_Header : IClassFixture<Delete_Without_Initiator_Header>
{
    private readonly Delete_Without_Initiator_Header _classFixture;

    public When_Removing_Contactgegeven_Without_Initiator_Header(Delete_Without_Initiator_Header classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_badRequest_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await _classFixture.Response.Content.ReadAsStringAsync();
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(body);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson("files.response.required");
}
