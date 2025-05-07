Feature: Creating a client
    Scenario: Successfully creating a client with valid credentials
        Given I have a client with name  and description 
        When the user sends the client creation request
        Then the response status should be Success
        And the client should be saved in the database