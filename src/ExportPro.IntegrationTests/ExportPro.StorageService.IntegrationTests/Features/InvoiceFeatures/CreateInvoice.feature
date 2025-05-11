@CreateInvoice
Feature: Creating a invoice after creating a currency, country and customer
    Scenario Outline: The User wants to create a invoice
        Given  The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The user has valid client id
        And The user created following currency for invoice and stored the currency id
          | CurrencyCode |
          | USD          |
        And The user created following currency for item and stored the currency id
          | CurrencyCode |
          | GBP          |
        And The user created following country and stored the country id
          | Name                      | Code                | CurrencyId |
          |TestUsa####TESTCUSTOMER### | TESTCOUNTRYCODECode |  temp      |
        And The user created following customer and stored the customer id
          | CountryId | Name                     | Email                              |
          |temp       | TESTUSER####TESTCUSTOMER | TESTUSER####TESTCUSTOMER@gmail.com |
        And The user wants to create following invoice
          | InvoiceNumber       | IssueDate  | DueDate    | CurrencyId | PaymentStatus | CustomerId | ClientId | ClientCurrencyId | 
          | 123456789#######000 | 2025-01-01 | 2025-01-31 | temp       | 0             | temp       | temp     | temp             | 
      And the invoice contains the following items
        | Name            | Description | Price | Status | CurrencyId |
        | ItemTESTInvoice | NAGARI      | 10.50 | 0      | temp       |
        When the user sends the invoice creation request
        Then  the invoice should be saved in the database
    Examples:
      | email                     | password          |
      | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
      | ClientAdminTest@gmail.com | ClientAdminTest2@ |
      | OperatorTest@gmail.com    | OperatorTest2@    |