using System.Text.Json;
using NUnit.Framework.Internal;

namespace prometheus_application_project;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class APITests
{
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = new();
        _client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
    }

    [Test]
    public async Task TestGet()
    {
        var response = await _client.GetAsync("/posts/1");
        var expected = """
            {
              "userId": 1,
              "id": 1,
              "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
              "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
            }
            """;
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(expected));
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestInvalidGet()
    {
        var response = await _client.GetAsync("/posts/1000000000");
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);

        response = await _client.GetAsync("/posts/-1");
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);

        response = await _client.GetAsync("/poats/1");
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestDelete()
    {
        var response = await _client.DeleteAsync("/posts/1");
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestInvalidDelete()
    {
        // It appears that jsonplaceholder still returns 200 on a valid path even if the resource doesn't exist.
        // I believe this is technically not accepted practice, but since I don't really have any contact with the developer I'm leaving these tests commented out for now.

        //Assert.That(!(await _client.DeleteAsync("/posts/1000000000")).IsSuccessStatusCode);

        //Assert.That(!(await _client.DeleteAsync("/posts/-1")).IsSuccessStatusCode);

        var response = await _client.DeleteAsync("/poats/1");
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestPost()
    {
        StringContent content = new(JsonSerializer.Serialize(new
        {
            userId = 1,
            title = "foo",
            body = "bar"
        }
        ));

        var expected = """
        {
          "id": 101
        }
        """;

        var response = await _client.PostAsync("/posts", content);
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(expected));
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestInvalidPost()
    {
        StringContent content = new(JsonSerializer.Serialize(new
        {
            userId = 1,
            title = "foo",
            body = "bar"
        }
        ));

        var response = await _client.PostAsync("/poats", content);
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);

        // It appears that post requests with improperly formatted data still go through.
        /*response = await _client.PostAsync("/posts", new StringContent("{}"));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);*/

    }

    [Test]
    public async Task TestPut()
    {
        StringContent content = new(JsonSerializer.Serialize(new
        {
            userId = 1,
            id = 1,
            title = "foo",
            body = "bar"
        }
        ));

        var expected = """
        {
          "id": 1
        }
        """;

        var response = await _client.PutAsync("/posts/1", content);
        Console.WriteLine(response);
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(expected));
        Assert.That(response.IsSuccessStatusCode);
    }

    [Test]
    public async Task TestInvalidPut()
    {
        StringContent content = new(JsonSerializer.Serialize(new
        {
            userId = 1,
            id = 1,
            title = "foo",
            body = "bar"
        }
        ));

        var response = await _client.PutAsync("/poats/1", content);
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);

        // It appears that put requests with improperly formatted data still go through.
        /*response = await _client.PutAsync("/posts/1", new StringContent("{}"));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{}"));
        Assert.That(!response.IsSuccessStatusCode);*/

    }
}