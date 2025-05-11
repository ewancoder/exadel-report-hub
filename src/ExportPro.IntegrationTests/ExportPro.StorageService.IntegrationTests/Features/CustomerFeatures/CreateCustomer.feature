@CreateCustomer
Feature: Creating a Customer
    Scenario Outline: The user creates a customer after creating a currency and country
        Given  The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The user created following currency and stored the currency id
            | CurrencyCode |
            | EUR          |
        And The user created following country and stored the country id
            | Name       | Code            | CurrencyId |
            |TestUsa####TESTCUSTOMER### | TESTCOUNTRYCODECode |  temp      |
        And The user wants to create following customer
            | CountryId | Name        | Email        | Address|
            |temp      | TESTUSER####TESTCUSTOMER | TESTUSER####TESTCUSTOMER@gmail.com | BERRLIN|
        When the user sends the customer creation request
        Then  the customer should be saved in the database
    Examples:
      | email                     | password          |
      | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
      | ClientAdminTest@gmail.com | ClientAdminTest2@ |
      | OperatorTest@gmail.com    | OperatorTest2@    |