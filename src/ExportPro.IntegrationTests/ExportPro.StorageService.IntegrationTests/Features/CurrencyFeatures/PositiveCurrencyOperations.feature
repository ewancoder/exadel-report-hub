@CurrencyManagement
Feature: Currency Management
    @CreateCurrency
    Scenario Outline: The user creates a currency
        Given The '<Role>' user is logged in with email and password and has necessary permissions
        And The user has following currency '<CurrencyCode>'
        When The user sends the currency creation request
        Then The currency should be saved in the database
        Examples: 
          | Role        | CurrencyCode |
          | Owner       | QQQ          |
          | ClientAdmin | PPP          |
          | Operator    | MMM          |
    @DeleteCurrency
    Scenario Outline: The user wants to delete a currency after creating a currency
        Given The '<Role>' user is logged in with email and password and has necessary permissions
        And The user has following currency '<CurrencyCode>' created
        When The user sends a delete request with currency id
        Then The currency should be deleted
        Examples: 
          | Role        | CurrencyCode |
          | Owner       | QQQ          |
          | ClientAdmin | PPP          |
          | Operator    | MMM          |