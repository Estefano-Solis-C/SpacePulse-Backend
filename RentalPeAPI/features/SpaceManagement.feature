Feature: Space Portfolio Management
  As an authenticated space owner
  I want to create, view, and publish spaces
  So that potential clients can see my available properties

  Scenario: Successfully creating a new space
    Given an authenticated user with an "Owner" role
    And valid space details including dimensions, photos, and space type
    When the user sends a POST request to create a space
    Then the response status code should be 201 Created
    And the new space should be assigned to the user's account

  Scenario: Listing all registered spaces for an authenticated user
    Given an authenticated user who has registered 3 spaces
    When the user sends a GET request to their spaces endpoint
    Then the response status code should be 200 OK
    And the response body should contain a list of 3 spaces with their full details

  Scenario: Publishing a space for public quotation
    Given an authenticated user with an unpublished space
    When the user sends a PATCH request to publish the space
    Then the response status code should be 200 OK
    And the space status should be updated to "Published"
    And the space should appear in the public public spaces list endpoint