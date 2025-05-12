@CreateCountry
Feature: Create a country

    Scenario Outline: The user creates a country after creating a currency
        Given The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The user created following currency and stored the currency id
          | CurrencyCode |
          | GBP          |
        And The user wants to create following country
          | Name        | Code            | CurrencyId |
          | TestUsa#### | TESTCOUNTRYCODE | temp       |
        When The user sends the country creation request
        Then The country should be saved in the database

        Examples:
          | email                     | password          |
          | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
          | ClientAdminTest@gmail.com | ClientAdminTest2@ |
          | OperatorTest@gmail.com    | OperatorTest2@    |