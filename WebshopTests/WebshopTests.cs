using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace WebshopTests
{
    public class WebshopTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        // Setup the Chromedriver, Opens webpage and logs in with the default user
        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Opens page and logs in
            _driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            var usernameField = _driver.FindElement(By.Id("user-name"));
            var passwordField = _driver.FindElement(By.Id("password"));
            var loginButton = _driver.FindElement(By.Id("login-button"));

            usernameField.SendKeys("standard_user");
            passwordField.SendKeys("secret_sauce");
            loginButton.Click();
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
        }

        // Logs out and checks if the Login button is present (it's on the login page)
        [Test]
        public void Test_Login_Logout()
        {
            var hamburgerButton = _driver.FindElement(By.Id("react-burger-menu-btn"));
            var logoutButton = _driver.FindElement(By.Id("logout_sidebar_link"));

            hamburgerButton.Click();
            logoutButton.Click();

            var loginButton = _driver.FindElement(By.Id("login-button"));

            Assert.That(loginButton, Is.Not.Null);
        }

        // Item added to the Cart and reads if the cart is NOT empty (Remove button exists)
        [Test]
        public void Items_In_The_Cart()
        {
            var addToCartButton = _driver.FindElement(By.ClassName("btn"));
            var shoppingCartButton = _driver.FindElement(By.Id("shopping_cart_container"));
            var removeFromCartButton = _driver.FindElement(By.ClassName("btn"));

            addToCartButton.Click();
            shoppingCartButton.Click();
            Assert.That(removeFromCartButton, Is.Not.Null);
        }

        // Item added to the Cart and than removed, checks if the cart page is empty
        [Test]
        public void Items_Removed_From_The_Cart()
        {
            // Add first item to the cart and opens it
            var addToCartButton = _driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack"));
            var shoppingCartButton = _driver.FindElement(By.Id("shopping_cart_container"));

            addToCartButton.Click();
            shoppingCartButton.Click();

            // Removes item from the cart
            var removeFromCartButton = _driver.FindElement(By.ClassName("btn"));
            removeFromCartButton.Click();

            // Checks if the REMOVE button NOT exists => cart is empty
            // This steps takes about 10-15sec, the driver waits for the element to appear
            try
            {
                var element = _driver.FindElement(By.Id("remove-sauce-labs-backpack"));
                // Fail test if element is found
                Assert.Fail("Element should not exist on the page.");
            }
            catch (NoSuchElementException)
            {
                // Passed Test if element is NOT found
                Assert.Pass("Element does not exist on the page.");
            }
        }

        // Checks if the sorting is working for A to Z 
        [Test]
        public void Sort_Items_On_MainPage_AtoZ()
        {
            // Finds the Sort dropdown
            var sortItems = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElement = new SelectElement(sortItems);

            // Select the A to Z option
            selectElement.SelectByValue("az");

            // Asserts if the dropdown is updated
            var selectedOption = selectElement.SelectedOption;
            Assert.That("Name (A to Z)" == selectedOption.Text, Is.True);

            // Gathers all the names of the products
            List<String> item = new List<string>();
            IReadOnlyList<IWebElement> itemList = (IReadOnlyList<IWebElement>)_driver.FindElements(By.ClassName("inventory_item_name"));

            // Checks if the elements of itemList are in alphabetical order
            Assert.That(IsSortedAlphabetically(itemList), Is.True);
        }

        private static bool IsSortedAlphabetically(IReadOnlyList<IWebElement> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                // Compare current item text with the next item text
                if (string.Compare(items[i].Text, items[i + 1].Text, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    return false; // Return false if items are out of order
                }
            }
            return true; // Return true if all items are in order
        }

        // Checks if the sorting is working for Z to A
        [Test]
        public void Sort_Items_On_MainPage_ZtoA()
        {
            //Finds the Sort dropdown
            var sortItems = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElement = new SelectElement(sortItems);

            // Select the Z to A option
            selectElement.SelectByValue("za");

            // Finds the new, sorted selection 
            var sortItemsZtoA = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElementZtoA = new SelectElement(sortItemsZtoA);

            // Asserts if the dropdown is updated
            var selectedOption = selectElementZtoA.SelectedOption;
            Assert.That("Name (Z to A)" == selectedOption.Text, Is.True);

            // Gathers all the names of the products
            List<String> item = new List<string>();
            IReadOnlyList<IWebElement> itemList = (IReadOnlyList<IWebElement>)_driver.FindElements(By.ClassName("inventory_item_name"));

            // Checks if the elements of itemList are in alphabetical DESC order
            Assert.That(IsSortedAlphabeticallyDesc(itemList), Is.True);
        }

        private static bool IsSortedAlphabeticallyDesc(IReadOnlyList<IWebElement> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                // Compare current item text with the next item text
                if (string.Compare(items[i].Text, items[i + 1].Text, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return false; // Return false if items are out of order
                }
            }
            return true; // Return true if all items are in order
        }

        // Checks if the sorting works for From Low to High price
        [Test]
        public void Sort_Items_On_MainPage_LoHi()
        {
            // Finds the Sort dropdown
            var sortItems = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElement = new SelectElement(sortItems);

            // Select the From Lowest to Highest
            selectElement.SelectByValue("lohi");

            // Finds the new, sorted selection 
            var sortItemsLoHi = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElementLoHi = new SelectElement(sortItemsLoHi);

            // Asserts if the dropdown is updated
            var selectedOption = selectElementLoHi.SelectedOption;
            Assert.That("Price (low to high)" == selectedOption.Text, Is.True);

            // Gathers all the names of the products
            List<String> item = new List<string>();
            IReadOnlyList<IWebElement> priceList = (IReadOnlyList<IWebElement>)_driver.FindElements(By.ClassName("inventory_item_price"));

            // Checks if the elements of priceList are in order
            Assert.That(IsSortedFromLowToHigh(priceList), Is.True);
        }

        private static bool IsSortedFromLowToHigh(IReadOnlyList<IWebElement> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                // Extract and parse the price values
                decimal currentPrice = ParsePrice(items[i].Text);
                decimal nextPrice = ParsePrice(items[i + 1].Text);

                // Compare current price with the next price
                if (currentPrice > nextPrice)
                {
                    return false; // Return false if items are out of order
                }
            }
            return true; // Return true if all items are in order
        }

        // Checks if the sorting works for From High to Low prices
        [Test]
        public void Sort_Items_On_MainPage_HiLo()
        {
            // Finds the Sort dropdown
            var sortItems = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElement = new SelectElement(sortItems);

            // Select the From Highest to Lowest
            selectElement.SelectByValue("hilo");

            // Finds the new, sorted selection 
            var sortItemsHiLo = _driver.FindElement(By.ClassName("product_sort_container"));
            var selectElementHitoLo = new SelectElement(sortItemsHiLo);

            // Asserts if the dropdown is updated
            var selectedOption = selectElementHitoLo.SelectedOption;
            Assert.That("Price (high to low)" == selectedOption.Text, Is.True);

            // Gathers all the prices of the products
            List<String> item = new List<string>();
            IReadOnlyList<IWebElement> priceList = (IReadOnlyList<IWebElement>)_driver.FindElements(By.ClassName("inventory_item_price"));

            // Checks if the elements of priceList are in DESC order
            Assert.That(IsSortedFromHighToLow(priceList), Is.True);
        }

        private static bool IsSortedFromHighToLow(IReadOnlyList<IWebElement> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                // Extract and parse the price values
                decimal currentPrice = ParsePrice(items[i].Text);
                decimal nextPrice = ParsePrice(items[i + 1].Text);

                // Compare current price with the next price
                if (currentPrice < nextPrice)
                {
                    return false; // Return false if items are out of order
                }
            }
            return true; // Return true if all items are in order
        }

        // Parsing the prices for the tests
        private static decimal ParsePrice(string priceText)
        {
            // Remove any currency symbols and convert the string to a decimal
            string priceWithoutSymbol = priceText.Replace("$", "").Trim();
            return decimal.Parse(priceWithoutSymbol, CultureInfo.InvariantCulture);
        }

        // Checks if the order process is working
        [Test]
        public void Ordering_Is_Successful()
        {
            // Add first item to the cart and opens it
            var addToCartButton = _driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack"));
            var shoppingCartButton = _driver.FindElement(By.Id("shopping_cart_container"));

            addToCartButton.Click();
            shoppingCartButton.Click();

            // Continues to the checkout screen
            var checkoutButton = _driver.FindElement(By.Id("checkout"));
            checkoutButton.Click();

            var firstNameField = _driver.FindElement(By.Id("first-name"));
            var lastNameField = _driver.FindElement(By.Id("last-name"));
            var postalCodeField = _driver.FindElement(By.Id("postal-code"));
            var continueButton = _driver.FindElement(By.Id("continue"));

            // Fills the required fields
            firstNameField.SendKeys("test_first_name");
            lastNameField.SendKeys("test_last_name");
            postalCodeField.SendKeys("test_postal_code");

            // Clicks on continue
            continueButton.Click();
            // Continues
            var finishButton = _driver.FindElement(By.Id("finish"));
            finishButton.Click();

            // Check if the checkout_complete_container is visible (contains the green checkmark icon, the text etc.)
            var checkoutCompleteContainer = _driver.FindElement(By.Id("checkout_complete_container"));
            Assert.That(checkoutCompleteContainer.Displayed, Is.True);
        }

        // Checks is the ordering process cannot be completed when the required fields are empty
        [Test]
        public void Checkout_Required_Fields()
        {
            // Add first item to the cart and opens it
            var addToCartButton = _driver.FindElement(By.Id("add-to-cart-sauce-labs-backpack"));
            var shoppingCartButton = _driver.FindElement(By.Id("shopping_cart_container"));

            addToCartButton.Click();
            shoppingCartButton.Click();

            // Continues to the checkout screen
            var checkoutButton = _driver.FindElement(By.Id("checkout"));
            checkoutButton.Click();

            var firstNameField = _driver.FindElement(By.Id("first-name"));
            var lastNameField = _driver.FindElement(By.Id("last-name"));
            var continueButton = _driver.FindElement(By.Id("continue"));

            // Clicks on continue without filling any fields
            continueButton.Click();

            // Checks if the error message is displayed
            var errorMessage = _driver.FindElement(By.ClassName("error-button"));
            Assert.That(errorMessage.Displayed, Is.True);

            // Fills first field, tries to continue, check if error msg is displayed
            firstNameField.SendKeys("test_first_name");
            continueButton.Click();
            Assert.That(errorMessage.Displayed, Is.True);

            // Fills first field, tries to continue, check if error msg is displayed
            lastNameField.SendKeys("test_last_name");
            continueButton.Click();
            Assert.That(errorMessage.Displayed, Is.True);
        }
    }
}

