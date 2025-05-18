@ClientManagement
Feature: Client Operations

    Background:
        Given The "SuperAdmin" user is logged in with email and password and has necessary permissions

    @CreateClient
    Scenario: The user creates a client
        And The user has a client with name and description
          | Name       | Description |
          | TestClient | Test Client |
        When the user sends the client creation request
        Then the client should be saved in the database

    @DeleteClient
    Scenario: The user wants to delete a client
        When The user sends a delete request with client id
        Then The client should be deleted