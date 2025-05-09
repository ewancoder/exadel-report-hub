Feature: Delete a country
  @DeleteCountry
    Scenario Outline: The user wants to delete a country after creating a currency and country
        Given  The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The following currency exists
          | CurrencyCode |
          | GBP          |
        And The country exists and the user has the country id
          | Name       | Code            | CurrencyId |
          |TestUsa#### | TESTCOUNTRYCODE |  temp      |
        When The user sends the country delete request
        Then The country should be deleted
        Examples:
          | email                     | password          |
          | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
          | ClientAdminTest@gmail.com | ClientAdminTest2@ |
          | OperatorTest@gmail.com    | OperatorTest2@    |