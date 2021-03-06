using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

//this a file that will house the multiple behav tests for our website

namespace SeleniumTests
{
    [TestFixture]
    public class MoveThroughWebsite
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
            baseURL = "https://www.katalon.com/";
            verificationErrors = new StringBuilder();
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        
        //go to the webpage and navigate to the "Patients" tab to bring up the list of patients
        [Test]
        public void NavigateToPatient()
        {
            driver.Navigate().GoToUrl("http://tumor1.azurewebsites.net/"); 
            Thread.Sleep(4500); 
            driver.FindElement(By.LinkText("Patients")).Click();
            Thread.Sleep(4500);
        }

        //go to the webpage and navigate to the "Compare" tab to bring up the list of patients
            //to compare to one another
        [Test]
        public void NavigateToCompare()
        {
            driver.Navigate().GoToUrl("http://tumor1.azurewebsites.net/");
            Thread.Sleep(4500);
            driver.FindElement(By.LinkText("Compare")).Click();
            Thread.Sleep(4500);
        }

        [Test]
        public void SearchPatientID()
        {
            driver.Navigate().GoToUrl("http://tumor1.azurewebsites.net/");
            Thread.Sleep(4500);
            driver.FindElement(By.LinkText("Patients")).Click();
            Thread.Sleep(4500);
            driver.FindElement(By.Name("q")).Click();
            driver.FindElement(By.Name("q")).Clear();
            driver.FindElement(By.Name("q")).SendKeys("3");
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            Thread.Sleep(4500);
            driver.FindElement(By.Name("q")).Click();
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            driver.FindElement(By.Id("buttonSubmit")).Click();
            driver.FindElement(By.Name("q")).Click();
            driver.FindElement(By.Name("q")).Clear();
            driver.FindElement(By.Name("q")).SendKeys("6");            
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            Thread.Sleep(4500);
            driver.FindElement(By.Name("q")).Click();
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            driver.FindElement(By.Id("buttonSubmit")).Click();
            driver.FindElement(By.Name("q")).Click();
            driver.FindElement(By.Name("q")).Clear();
            driver.FindElement(By.Name("q")).SendKeys("7");
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            Thread.Sleep(4500);
        }

        [Test]
        public void SearchPatientClass()
        {

        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}
