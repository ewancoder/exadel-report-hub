Feature: Delete a currency
    Scenario: The user wants to delete a currency
        Given The user has a valid token for deleting
        And The user has currency id 
        When The user sends the currency delete request
        Then The currency should be deleted 