Feature: Create a currency
    Scenario: The user wants to create a currency
        Given The user have a currency 
        And The user have a valid token
        When the user sends the currency creation request 
        Then the response status should be Success
        And the currency should be saved in the database