Feature: Automated Monitoring Tasks
  As an authenticated user managing IoT Devices
  I want to configure automated monitoring tasks
  So that I can track space conditions periodically without manual intervention

  Scenario: Successfully creating a monitoring task for an IoT device
    Given an authenticated user managing a registered IoT device
    And the user provides valid task details including a monitoring schedule and task objective
    When the user sends a POST request to the create work item endpoint
    Then the response status code should be 201 Created
    And the task should be correctly scheduled in the database