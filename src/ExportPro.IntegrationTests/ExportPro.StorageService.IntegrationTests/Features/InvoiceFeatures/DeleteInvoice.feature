@DeleteInvoice
Feature: Delete an invoice
    Scenario: The user wants to delete an invoice after creating a currency, country, and invoice
        Given The user is logged in with the following credentials and has necessary permissions
          | Email                   | Password        |
          | OwnerUserTest@gmail.com | OwnerUserTest2@ |
         And the user has valid client id
        And The user created the following currency for invoice and stored the currency id
          | CurrencyCode |
          | GBP          |
        And The user created the following currency for item and stored the currency id
          | CurrencyCode |
          | USD          |
        And The user created the following country and stored the country id
          | Name                       | Code                | CurrencyId |
          | TestUsa####TESTCUSTOMER### | TESTCOUNTRYCODECode | temp       |
        And The user created the following customer and stored the customer id
          | CountryId | Name                     | Email                              |Address| 
          | temp      | TESTUSER####TESTCUSTOMER | TESTUSER####TESTCUSTOMER@gmail.com |dubai|
        And The user has the following invoice
          | InvoiceNumber       | IssueDate  | DueDate    | CurrencyId | PaymentStatus | CustomerId | ClientId | ClientCurrencyId |
          | 123456789#######000InvoiceDelete | 2025-01-01 | 2025-01-31 | temp       | 0             | temp       | temp     | temp             |
        And the invoice contains the following items and the invoice id is stored
          | Name            | Description | Price | Status | CurrencyId |
          | ItemTESTInvoice | NAGARI      | 10.50 | 0      | temp       |
        When The user sends the invoice delete request
        Then The invoice should be deleted