Feature: Delete a client
    @DeleteClient
    Scenario: Deleting a client with valid creditinals
        Given The user is logged in with email  and password  and has necessary permissions
          | Email                    | Password         |
          | SuperAdminTest@gmail.com | SuperAdminTest2@ |
        And the client exists and The user has a client id 
            | Name        | Description |
            | TESTCLIENT#####DELETE| Test Client |
        When The user send a delete request 
        Then The client should be deleted
        
        