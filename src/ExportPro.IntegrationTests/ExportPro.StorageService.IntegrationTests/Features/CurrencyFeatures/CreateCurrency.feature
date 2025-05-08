Feature: Create a currency
    Scenario: The user wants to create a currency
        Given The user has a valid token for creating
        And The user has a currency 
        When The user sends the currency creation request 
        Then The currency should be saved in the database