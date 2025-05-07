Feature: Creating a client
    Scenario: With valid credentials
        Given the user enters the client
        When I send the client creation request
        Then the response status should be 201
        And the client should be saved