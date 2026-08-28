using TechTalk.SpecFlow;
using Xunit;

namespace RentalPeAPI.Steps
{
    [Binding]
    public class AutomatedTasksSteps
    {
        [Given(@"an authenticated user managing a registered IoT device")]
        public void GivenAnAuthenticatedUserManagingARegisteredIotDevice()
        {
        }

        [Given(@"the user provides valid task details including a monitoring schedule and task objective")]
        public void GivenTheUserProvidesValidTaskDetails()
        {
        }

        [When(@"the user sends a POST request to the create work item endpoint")]
        public void WhenTheUserSendsAPostRequestToTheCreateWorkItemEndpoint()
        {
        }

        [Then(@"the task should be correctly scheduled in the database")]
        public void ThenTheTaskShouldBeCorrectlyScheduledInTheDatabase()
        {
        }
    }
}