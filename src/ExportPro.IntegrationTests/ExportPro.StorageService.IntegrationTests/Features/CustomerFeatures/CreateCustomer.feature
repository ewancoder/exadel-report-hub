Feature: Creating a Customer
    Scenario: The User wants to create a customer
        Given The user has a valid token for creating customer
        And The user has a customer to create
        When the user sends the customer creation request
        Then  the customer should be saved in the database