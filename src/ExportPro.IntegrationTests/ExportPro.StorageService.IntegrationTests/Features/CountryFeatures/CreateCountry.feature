Feature: Create a country
    Scenario Outline: The user creates a country
      Given  The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The following currency exists
          | CurrencyCode |
          | GBP          |
        And The user has a country to create
          | Name       | Code            | CurrencyId |
          |TestUsa#### | TESTCOUNTRYCODE |  temp      |
        When The user sends the country creation request 
        Then The country should be saved in the database
        Examples:
          | email                     | password          |
          | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
          | ClientAdminTest@gmail.com | ClientAdminTest2@ |
          | OperatorTest@gmail.com    | OperatorTest2@    |