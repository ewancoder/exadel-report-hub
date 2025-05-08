Feature: Delete a country
    Scenario: The user wants to delete a country
        Given The user has a valid token for deleting
        And The user has country id 
        When The user sends the country delete request
        Then The country should be deleted