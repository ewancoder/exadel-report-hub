Feature: Create a country
    Scenario: The user wants to create a country
        Given The user has a valid token for creating
        And The user has a country 
        When The user sends the country creation request 
        Then The country should be saved in the database