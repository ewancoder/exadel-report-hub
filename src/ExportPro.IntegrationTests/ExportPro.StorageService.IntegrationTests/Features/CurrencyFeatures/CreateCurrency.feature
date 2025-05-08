Feature: Create a currency
    Scenario: The user wants to create a currency
        Given The user have a currency 
        And The user has a valid token
        When The user sends the currency creation request 
        Then The currency should be saved in the database