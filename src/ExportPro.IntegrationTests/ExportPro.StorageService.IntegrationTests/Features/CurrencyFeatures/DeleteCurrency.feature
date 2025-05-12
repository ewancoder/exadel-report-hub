@DeleteCurrency
Feature: Delete a currency

    Scenario Outline: The user wants to delete a currency after creating a currency
        Given The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And the user has created the following currency and stored the currency id
          | CurrencyCode |
          | QQQ          |
        When The user sends the currency delete request
        Then The currency should be deleted

        Examples:
          | email                     | password          |
          | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
          | ClientAdminTest@gmail.com | ClientAdminTest2@ |
          | OperatorTest@gmail.com    | OperatorTest2@    |