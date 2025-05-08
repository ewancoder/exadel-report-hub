Feature: Delete a client
    Scenario: Deleting a client with valid creditinals
        Given  User have a valid token 
        And User have a client id 
        When User send a delete request 
        Then The client should be deleted
        
        