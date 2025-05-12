@CreateClient
Feature: Creating a client

    Scenario: The user creates a client
        Given The user is logged in with email  and password  and has necessary permissions
          | Email                    | Password         |
          | SuperAdminTest@gmail.com | SuperAdminTest2@ |
        And The user have a client with name and description
          | Name       | Description |
          | TestClient | Test Client |
        When the user sends the client creation request
        Then the client should be saved in the database