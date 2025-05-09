@DeleteCustomer
Feature: Delete a customer
    Scenario: The user wants to delete a customer after creating a currency , country and customer
        Given The user is logged in with email and password and has necessary permissions
          | Email                   | Password        |
          | OwnerUserTest@gmail.com | OwnerUserTest2@ |
        And The user has created following currency and stored the currency id
            | CurrencyCode |
            | EUR          |
        And The user has created following country and stored the country id
            | Name       | Code            | CurrencyId |
            |TestUsa####TESTCUSTOMER###TESTCUSTOMER | TESTCOUNTRYCODECodePPPPPPP |  temp      |
        And The user has created following customer and stored the customer id
            | CountryId | Name        | Email        |
            |temp      | TESTUSER####TESTCUSTOMER | TESTUSER####TESTCUSTOMER@gmail.com |
        When The user sends the customer delete request
        Then The customer should be deleted 
