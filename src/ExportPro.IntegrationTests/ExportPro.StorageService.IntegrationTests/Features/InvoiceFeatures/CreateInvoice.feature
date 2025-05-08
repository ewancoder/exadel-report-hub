Feature: Creating a invoice
    Scenario: The User wants to create a invoice
        Given The user has a valid token for creating
        And The user has a invoice to create
        When the user sends the invoice creation request
        Then  the user should be saved in the database