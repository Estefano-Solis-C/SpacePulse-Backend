Feature: User Identity and Access Management
  As a new or returning user of SpacePulse
  I want to register, log in, and manage my session securely
  So that I can access the platform functionalities based on my role

  Scenario: Successful user registration with role assignment
    Given the user provides valid registration details with email "test@spacepulse.com", password "SecurePass123!", and role "Homeowner"
    When the user sends a POST request to the registration endpoint
    Then the response status code should be 201 Created
    And the user should be saved in the database with the "Owner" role

  Scenario: Failed registration due to duplicate email
    Given a user with the email "test@spacepulse.com" already exists in the system
    When the user sends a POST request to the registration endpoint with email "test@spacepulse.com"
    Then the response status code should be 400 Bad Request
    And the response should contain an error message indicating the email is already in use

  Scenario: Successful user login and JWT generation
    Given a registered user with email "test@spacepulse.com" and password "SecurePass123!"
    When the user sends a POST request to the login endpoint with valid credentials
    Then the response status code should be 200 OK
    And the response body should contain a valid JWT token

  Scenario: Failed login due to invalid credentials
    Given a registered user with email "test@spacepulse.com"
    When the user sends a POST request to the login endpoint with an incorrect password
    Then the response status code should be 401 Unauthorized
    And the response should contain an error message about invalid credentials