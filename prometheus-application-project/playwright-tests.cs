using Microsoft.Playwright;
using NUnit.Framework.Internal;

namespace prometheus_application_project;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    [Test]
    public async Task TryContactPrometheus()
    {
        // I'm running in non-headless mode because google is hitting me with their captcha.
        await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://google.com/");

        await page.GetByRole(AriaRole.Combobox, new() { NameString = "Search" }).FillAsync("Prometheus Group");
        
        // For some reason. this needs to be forced or it will time out. It appears that there is another subtree that intercepts pointer events.
        await page.GetByRole(AriaRole.Button, new() { NameString = "Google Search" }).ClickAsync(new() { Force = true });

        await page.WaitForURLAsync("https://www.google.com/sorry/**");
        // IMPORTANT: user must open the headed browser and manually complete the captcha at this point.
        await page.WaitForURLAsync("https://www.google.com/search**");

        Assert.That(await page.GetByRole(AriaRole.Link).GetByText("Prometheus Group").CountAsync(), Is.GreaterThanOrEqualTo(1));

        var link = page.GetByRole(AriaRole.Link).GetByText("Contact Us");

        // Normally, I would have all this in one line, but since Google is a third party website and search results could potentially change unpredictably, it makes sense to validate the contact link as well.
        await Expect(link).ToHaveCountAsync(1);

        await link.ClickAsync();

        await page.WaitForURLAsync("https://www.prometheusgroup.com/contact-us");

        await page.GetByLabel(new Regex("first name", RegexOptions.IgnoreCase)).FillAsync("Charlie");
        await page.GetByLabel(new Regex("last name", RegexOptions.IgnoreCase)).FillAsync("Lomax");
        await page.GetByRole(AriaRole.Button, new() { NameString = "Contact Us" }).ClickAsync(new() { Force = true });

        // This locator finds all the elements with the 'required' attribute
        await Expect(page.Locator("[required]")).ToHaveCountAsync(6);
    }
}
