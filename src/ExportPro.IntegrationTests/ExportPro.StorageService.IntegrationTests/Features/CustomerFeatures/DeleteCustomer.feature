Feature: Delete a customer
    Scenario: The user wants to delete a customer
        Given The user has a valid token for deleting customer
        And The user has customer id 
        When The user sends the customer delete request
        Then The customer should be deleted 