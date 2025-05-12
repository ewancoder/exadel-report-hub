@DeleteClient
Feature: Delete a client

    Scenario: The user wants to delete a client after creating a client
        Given The user is logged in with email  and password  and has necessary permissions
          | Email                    | Password         |
          | SuperAdminTest@gmail.com | SuperAdminTest2@ |
        And The user creates a client and The stores the client id
          | Name                  | Description |
          | TESTCLIENT#####DELETE | Test Client |
        When The user send a delete request
        Then The client should be deleted