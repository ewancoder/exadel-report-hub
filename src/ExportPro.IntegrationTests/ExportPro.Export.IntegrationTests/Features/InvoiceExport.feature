@InvoiceExport
Feature: Exporting a invoice 
    Scenario Outline: The user wants to export an invoice as pdf after creating invoice and its models
        Given The user is logged in with email '<email>' and password '<password>' and has necessary permissions
        And The user has valid client id
        And The user created following currency for invoice and stored the currency id
          | CurrencyCode |
          | USD          |
        And The user created following currency for item and stored the currency id
          | CurrencyCode |
          | GBP          |
        And The user created following country and stored the country id
          | Name                      | Code                | CurrencyId |
          |TestUsa####TESTCUSTOMER###lll | TESTCOUNTRYCODECodeP |  temp      |
        And The user created following customer and stored the customer id
          | CountryId | Name                     | Email                              | Address |
          |temp       | TESTUSER####TESTCUSTOMER]]]]] | TESTUSERpppTESTCUSTOMER@gmail.com | TBILISINAZALADEVI |
        And The user wants to create following invoice
          | InvoiceNumber       | IssueDate  | DueDate    | CurrencyId | PaymentStatus | CustomerId | ClientId | ClientCurrencyId | 
          | 123456789#######0005552 | 2025-01-01 | 2025-01-31 | temp       | 0             | temp       | temp     | temp             | 
        And the invoice contains the following items
          | Name            | Description | Price | Status | CurrencyId |
          | ItemTESTInvoice | NAGARI      | 10.50 | 0      | temp       |
        And The user created invoice and  stored invoice id 
        When The user sends the invoice export request
        Then The invoice should be exported as pdf
        Examples:
          | email                     | password          |
          | OwnerUserTest@gmail.com   | OwnerUserTest2@   |
          | ClientAdminTest@gmail.com | ClientAdminTest2@ |
          | OperatorTest@gmail.com    | OperatorTest2@    |
