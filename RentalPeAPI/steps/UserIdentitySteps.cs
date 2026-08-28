using TechTalk.SpecFlow;
using Xunit;

namespace RentalPeAPI.Steps
{
    [Binding]
    public class UserIdentitySteps
    {
        [Given(@"the user provides valid registration details with email ""([^""]*)"", password ""([^""]*)"", and role ""([^""]*)""")]
        public void GivenTheUserProvidesValidRegistrationDetails(string email, string password, string role)
        {
        }

        [When(@"the user sends a POST request to the registration endpoint")]
        public void WhenTheUserSendsAPostRequestToTheRegistrationEndpoint()
        {
        }

        [Then(@"the response status code should be (.*) Created")]
        public void ThenTheResponseStatusCodeShouldBeCreated(int statusCode)
        {
        }

        [Then(@"the user should be saved in the database with the ""([^""]*)"" role")]
        public void ThenTheUserShouldBeSavedInTheDatabaseWithTheRole(string role)
        {
        }

        [Given(@"a user with the email ""([^""]*)"" already exists in the system")]
        public void GivenAUserWithTheEmailAlreadyExistsInTheSystem(string email)
        {
        }

        [When(@"the user sends a POST request to the registration endpoint with email ""([^""]*)""")]
        public void WhenTheUserSendsAPostRequestToTheRegistrationEndpointWithEmail(string email)
        {
        }

        [Then(@"the response status code should be (.*) Bad Request")]
        public void ThenTheResponseStatusCodeShouldBeBadRequest(int statusCode)
        {
        }

        [Then(@"the response should contain an error message indicating the email is already in use")]
        public void ThenTheResponseShouldContainAnErrorMessage()
        {

        }

        [Given(@"a registered user with email ""([^""]*)"" and password ""([^""]*)""")]
        public void GivenARegisteredUserWithEmailAndPassword(string email, string password)
        {

        }

        [When(@"the user sends a POST request to the login endpoint with valid credentials")]
        public void WhenTheUserSendsAPostRequestToTheLoginEndpointWithValidCredentials()
        {

        }

        [Then(@"the response status code should be (.*) OK")]
        public void ThenTheResponseStatusCodeShouldBeOK(int statusCode)
        {

        }

        [Then(@"the response body should contain a valid JWT token")]
        public void ThenTheResponseBodyShouldContainAValidJwtToken()
        {

        }

        [Given(@"a registered user with email ""([^""]*)""$")]
        public void GivenARegisteredUserWithEmail(string email)
        {

        }

        [When(@"the user sends a POST request to the login endpoint with an incorrect password")]
        public void WhenTheUserSendsAPostRequestToTheLoginEndpointWithAnIncorrectPassword()
        {

        }

        [Then(@"the response status code should be (.*) Unauthorized")]
        public void ThenTheResponseStatusCodeShouldBeUnauthorized(int statusCode)
        {

        }

        [Then(@"the response should contain an error message about invalid credentials")]
        public void ThenTheResponseShouldContainAnErrorMessageAboutInvalidCredentials()
        {

        }
    }
}