using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechTalk.SpecFlow;
 

namespace RL.AutomatedTests.Steps.Plan;

[Binding]
public class PlanSteps
{
    private readonly ScenarioContext _context;
    private readonly string _urlBase = "http://localhost:3001";
    private readonly TimeSpan _waitDurration = new(0, 0, 1);

    public PlanSteps(ScenarioContext context)
    {
        _context = context;
    }

    [Given("I'm on the start page")]
    public async Task ImOnTheStartPage()
    {
        var driver = _context.Get<IWebDriver>("driver");
        driver.Navigate().GoToUrl(_urlBase);
        var wait = new WebDriverWait(driver, _waitDurration);
        wait.Until(ExpectedConditions.UrlContains(_urlBase));
    }

    [When("I click on start")]
    public async Task IClickOnStart()
    {
        var driver = _context.Get<IWebDriver>("driver");
        driver.FindElement(By.Id("start")).Click();
        var wait = new WebDriverWait(driver, _waitDurration);
        wait.Until(ExpectedConditions.UrlMatches(_urlBase + "/plan"));
    }

    [Then("I'm on the plan page")]
    public async Task ImOnThePlanPage()
    {
        var driver = _context.Get<IWebDriver>("driver");
        var wait = new WebDriverWait(driver, _waitDurration);
        wait.Until(ExpectedConditions.UrlMatches(@"/plan/(\d+)"));
        Thread.Sleep(10000);
        driver.Url.Should().MatchRegex(@"/plan/(\d+)");
    }

    [Then("I'm adding procedure")]
    public async Task AddProcedure()
    {
        var driver = _context.Get<IWebDriver>("driver");
        driver.FindElement(By.XPath("//*[@id='root']/div/div/div[2]/div/div/div/div/div[1]/div/div[1]/div/input")).Click();
        var wait = new WebDriverWait(driver, _waitDurration);
        Thread.Sleep(10000);
    }

    [Then("I'm assigning user")]
    public async Task AssignUser()
    {
        var driver = _context.Get<IWebDriver>("driver");
        driver.FindElement(By.Id("react-select-3-input")).SendKeys("Nick Morrison");
        driver.FindElement(By.Id("react-select-3-input")).SendKeys(Keys.Return);
        Thread.Sleep(10000);
    }

    [Then("I'm refereshing the page")]
    public async Task RefreshPage()
    {
        var driver = _context.Get<IWebDriver>("driver");
        driver.Navigate().Refresh();
        Thread.Sleep(10000);

    }

    [Then("I'm checking the user name")]
    public async Task Checkusername()
    {
        var driver = _context.Get<IWebDriver>("driver");
        String actualName = driver.FindElement(By.XPath("//*[@id='root']/div/div/div[2]/div/div/div/div/div[2]/div/div/div[2]/div/div[1]/div[1]/div[1]")).Text;
        Assert.AreEqual("Nick Morrison", actualName, "Expected name is not observed");
    }
}