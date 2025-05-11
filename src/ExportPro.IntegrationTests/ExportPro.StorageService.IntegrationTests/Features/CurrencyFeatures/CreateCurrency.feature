Feature: Create a currency
@CreateCurrency
    Scenario Outline: The user creates a currency
        Given The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The user has a currency 
          | CurrencyCode |
          | ZZZ          |
        When The user sends the currency creation request 
        Then The currency should be saved in the database
    Examples:
      | email                     | password          |
      | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
      | ClientAdminTest@gmail.com | ClientAdminTest2@ |
      | OperatorTest@gmail.com    | OperatorTest2@    |
