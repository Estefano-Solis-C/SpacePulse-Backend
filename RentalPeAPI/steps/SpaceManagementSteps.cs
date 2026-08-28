using TechTalk.SpecFlow;
using Xunit;

namespace RentalPeAPI.Steps
{
    [Binding]
    public class SpaceManagementSteps
    {
        [Given(@"an authenticated user with an ""(.*)"" role")]
        public void GivenAnAuthenticatedUserWithAnRole(string role)
        {
        }

        [Given(@"valid space details including dimensions, photos, and space type")]
        public void GivenValidSpaceDetails()
        {
        }

        [When(@"the user sends a POST request to create a space")]
        public void WhenTheUserSendsAPostRequestToCreateASpace()
        {
        }

        [Then(@"the new space should be assigned to the user's account")]
        public void ThenTheNewSpaceShouldBeAssignedToTheUsersAccount()
        {
        }

        [Given(@"an authenticated user who has registered (.*) spaces")]
        public void GivenAnAuthenticatedUserWhoHasRegisteredSpaces(int spaceCount)
        {
        }

        [When(@"the user sends a GET request to their spaces endpoint")]
        public void WhenTheUserSendsAGetRequestToTheirSpacesEndpoint()
        {
        }

        [Then(@"the response body should contain a list of (.*) spaces with their full details")]
        public void ThenTheResponseBodyShouldContainAListOfSpaces(int expectedCount)
        {
        }

        [Given(@"an authenticated user with an unpublished space")]
        public void GivenAnAuthenticatedUserWithAnUnpublishedSpace()
        {

        }

        [When(@"the user sends a PATCH request to publish the space")]
        public void WhenTheUserSendsAPatchRequestToPublishTheSpace()
        {
        }

        [Then(@"the space status should be updated to ""(.*)""")]
        public void ThenTheSpaceStatusShouldBeUpdatedTo(string expectedStatus)
        {
        }

        [Then(@"the space should appear in the public public spaces list endpoint")]
        public void ThenTheSpaceShouldAppearInThePublicList()
        {
        }
    }
}