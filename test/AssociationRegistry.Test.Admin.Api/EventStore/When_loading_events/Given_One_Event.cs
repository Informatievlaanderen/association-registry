namespace AssociationRegistry.Test.Admin.Api.EventStore.When_loading_events;

using Fixtures;
using When_storing_an_event;
using Xunit;

public class Given_One_Event_Fixture : AdminApiFixture
{
    public Given_One_Event_Fixture() : base(nameof(Given_One_Event_Fixture))
    {
    }
}

public class Given_One_Event:IClassFixture<Given_One_Event_Fixture>
{
    private readonly AdminApiFixture _fixture;

    public Given_One_Event(Given_An_Event_Fixture fixture)
    {
        _fixture = fixture;
    }


}
