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
        await Page.GotoAsync("https://google.com");

        await Page.GetByRole(AriaRole.Combobox, new() { NameString = "Search" }).FillAsync("Prometheus Group");

        // For some reason. this needs to be forced or it will time out. It appears that there is another subtree that intercepts pointer events.
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Google Search" }).ClickAsync(new() { Force = true });

        await Page.WaitForURLAsync("**");

        Console.WriteLine(Page.Url);

        // I keep getting hit with Google's Captcha, and I can't find a way to bypass it that doesn't cost money and that works with the C# version of Playwright, which renders this entire section unrunnable.
        /*await Page.GotoAsync("https://www.google.com/search?q=prometheus+group");

        await Page.WaitForURLAsync("**");
        Console.WriteLine(Page.Url);

        Assert.That(await Page.GetByRole(AriaRole.Link).GetByText("Prometheus Group").CountAsync(), Is.GreaterThanOrEqualTo(1));

        var link = Page.GetByRole(AriaRole.Link).GetByText("Contact Us");

        // Normally, I would have all this in one line, but since Google is a third party website and search results could potentially change unpredictably, it makes sense to validate the contact link as well.
        await Expect(link).ToHaveCountAsync(1);

        await link.ClickAsync();

        await Page.WaitForNavigationAsync();

        Console.WriteLine(Page.Url);*/

        // Dodging google's captcha
        await Page.GotoAsync("https://www.prometheusgroup.com/contact-us");

        await Page.WaitForURLAsync("https://www.prometheusgroup.com/contact-us");

        await Page.GetByLabel(new Regex("first name", RegexOptions.IgnoreCase)).FillAsync("Charlie");
        await Page.GetByLabel(new Regex("last name", RegexOptions.IgnoreCase)).FillAsync("Lomax");
        await Page.GetByRole(AriaRole.Button, new() { NameString = "Contact Us" }).ClickAsync(new() { Force = true });

        // This locator finds all the elements with the 'required' attribute
        await Expect(Page.Locator("[required]")).ToHaveCountAsync(6);
    }
}
