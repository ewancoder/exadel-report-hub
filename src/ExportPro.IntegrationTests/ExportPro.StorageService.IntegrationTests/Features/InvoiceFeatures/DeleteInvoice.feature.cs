﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ExportPro.StorageService.IntegrationTests.Features.InvoiceFeatures
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Delete an invoice")]
    [NUnit.Framework.CategoryAttribute("DeleteInvoice")]
    public partial class DeleteAnInvoiceFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "DeleteInvoice"};
        
#line 1 "DeleteInvoice.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/InvoiceFeatures", "Delete an invoice", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The user wants to delete an invoice after creating a currency, country, and invoi" +
            "ce")]
        public void TheUserWantsToDeleteAnInvoiceAfterCreatingACurrencyCountryAndInvoice()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The user wants to delete an invoice after creating a currency, country, and invoi" +
                    "ce", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 4
    this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table19 = new TechTalk.SpecFlow.Table(new string[] {
                            "Email",
                            "Password"});
                table19.AddRow(new string[] {
                            "OwnerUserTest@gmail.com",
                            "OwnerUserTest2@"});
#line 5
        testRunner.Given("The user is logged in with the following credentials and has necessary permission" +
                        "s", ((string)(null)), table19, "Given ");
#line hidden
#line 8
        testRunner.And("the user has valid client id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                            "CurrencyCode"});
                table20.AddRow(new string[] {
                            "GBP"});
#line 9
        testRunner.And("The user created the following currency for invoice and stored the currency id", ((string)(null)), table20, "And ");
#line hidden
                TechTalk.SpecFlow.Table table21 = new TechTalk.SpecFlow.Table(new string[] {
                            "CurrencyCode"});
                table21.AddRow(new string[] {
                            "USD"});
#line 12
        testRunner.And("The user created the following currency for item and stored the currency id", ((string)(null)), table21, "And ");
#line hidden
                TechTalk.SpecFlow.Table table22 = new TechTalk.SpecFlow.Table(new string[] {
                            "Name",
                            "Code",
                            "CurrencyId"});
                table22.AddRow(new string[] {
                            "TestUsa####TESTCUSTOMER###",
                            "TESTCOUNTRYCODECode",
                            "temp"});
#line 15
        testRunner.And("The user created the following country and stored the country id", ((string)(null)), table22, "And ");
#line hidden
                TechTalk.SpecFlow.Table table23 = new TechTalk.SpecFlow.Table(new string[] {
                            "CountryId",
                            "Name",
                            "Email",
                            "Address"});
                table23.AddRow(new string[] {
                            "temp",
                            "TESTUSER####TESTCUSTOMER",
                            "TESTUSER####TESTCUSTOMER@gmail.com",
                            "dubai"});
#line 18
        testRunner.And("The user created the following customer and stored the customer id", ((string)(null)), table23, "And ");
#line hidden
                TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
                            "InvoiceNumber",
                            "IssueDate",
                            "DueDate",
                            "CurrencyId",
                            "PaymentStatus",
                            "CustomerId",
                            "ClientId",
                            "ClientCurrencyId"});
                table24.AddRow(new string[] {
                            "123456789#######000InvoiceDelete",
                            "2025-01-01",
                            "2025-01-31",
                            "temp",
                            "0",
                            "temp",
                            "temp",
                            "temp"});
#line 21
        testRunner.And("The user has the following invoice", ((string)(null)), table24, "And ");
#line hidden
                TechTalk.SpecFlow.Table table25 = new TechTalk.SpecFlow.Table(new string[] {
                            "Name",
                            "Description",
                            "Price",
                            "Status",
                            "CurrencyId"});
                table25.AddRow(new string[] {
                            "ItemTESTInvoice",
                            "NAGARI",
                            "10.50",
                            "0",
                            "temp"});
#line 24
        testRunner.And("the invoice contains the following items and the invoice id is stored", ((string)(null)), table25, "And ");
#line hidden
#line 27
        testRunner.When("The user sends the invoice delete request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 28
        testRunner.Then("The invoice should be deleted", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
