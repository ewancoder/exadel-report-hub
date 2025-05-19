@CreateCountry
Feature: Create a country

    Scenario Outline: The user creates a country after creating a currency
        Given The '<Role>' user is logged in with email and password and has necessary permissions
        And The user created following currency and stored the currency id
          | CurrencyCode |
          | GBP          |
        And The user wants to create following country
          | Name        | Code            | CurrencyId |
          | TestUsa#### | TESTCOUNTRYCODE | temp       |
        When The user sends the country creation request
        Then The country should be saved in the database

        Examples:
          | Role              |
          | Owner             | 
          | ClientAdmin | 
          | Operator| 