Feature: Delete a invoice
    Scenario: The user wants to delete a invoice
        Given The user has a valid token for deleting
        And The user has invoice id 
        When The user sends the invoice delete request
        Then The invoice should be deleted